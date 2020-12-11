using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class BaseManager
	{
		protected int packSize = 500;
		protected int threadCount = 20;
		protected string _tableName;
		protected string _processingPrimaryMethodName;
		protected string _processingMethodName;
		protected bool _isNeedSendToProcessing;
		protected bool _isNeedSendToPersonalArea;
		protected object _lock = new object();
		protected object _lockRes = new object();

		public BaseManager()
		{
			GlobalCacheReader.GetValue<int>(GlobalCacheReader.CacheKeys.PackSize, out packSize);
			GlobalCacheReader.GetValue<int>(GlobalCacheReader.CacheKeys.ThreadCount, out threadCount);
		}

		protected virtual List<BaseIntegrationObject> ReadPack(bool isPC)
		{
			return new List<BaseIntegrationObject>();
		}

		public static void CreateColumns() 
		{
			DBConnectionProvider.ExecuteNonQuery(@"IF (SELECT 1 FROM sys.columns 
							  WHERE Name = N'StatusPC'
							  AND Object_ID = Object_ID(N'Contact'))  is null
					BEGIN
						ALTER TABLE Contact ADD StatusPC int not null default 0
					END

					IF (SELECT 1 FROM sys.columns 
							  WHERE Name = N'StatusCS'
							  AND Object_ID = Object_ID(N'Contact')) is null
					BEGIN
						ALTER TABLE Contact ADD StatusCS int not null default 0
					END
			
					IF (SELECT 1 FROM sys.columns 
							  WHERE Name = N'ErrorMessage'
							  AND Object_ID = Object_ID(N'Contact')) is null
					BEGIN
						ALTER TABLE Contact ADD ErrorMessage nvarchar(max) null
					END
			");
		}

		public virtual void Execute(bool isNeedSendToPersonalArea)
		{
			Logger.LogInfo("Начался импорт", _tableName);

			var tasks = new List<Task>();
			for (var j = 0; j < threadCount; j++)
			{
				var task = new Task(() =>
				{
					var pack = new List<BaseIntegrationObject>();
					try
					{
						pack = ReadPack(!isNeedSendToPersonalArea);
						Logger.LogInfo(string.Format("Прочитано данных из {0}: {1}", _tableName, pack.Count), "");
					}
					catch (Exception e)
					{
						Logger.LogError(string.Format("Ошибка чтения данных из {0}", _tableName), e);
					}

					while (pack.Count > 0)
					{
						var now = DateTime.Now;

						var isProcessingSuccess = false;
						PackResults results = null;
						try
						{
							isProcessingSuccess = SendToProcessing(pack, out now, out results, isNeedSendToPersonalArea);
						}
						catch (Exception e)
						{
							Logger.LogError("SendToProcessing error ", e);
						}

						ProceedResults(results, isNeedSendToPersonalArea);

						Logger.LogInfo(_tableName, "pack finished");
						pack = new List<BaseIntegrationObject>();
						try
						{
							pack = ReadPack(!isNeedSendToPersonalArea);
							Logger.LogInfo(string.Format("Прочитано данных из {0}: {1}", _tableName, pack.Count), "");
						}
						catch (Exception e)
						{
							Logger.LogError(string.Format("Ошибка чтения данных из {0}", _tableName), e);
						}
					}
				});

				task.Start();
				tasks.Add(task);
			}

			Task.WaitAll(tasks.ToArray());

			Logger.LogInfo("Finished", _tableName);
		}

		private bool SendToProcessing(List<BaseIntegrationObject> pack, out DateTime now, out PackResults results, bool isUsePA)
		{
			now = DateTime.Now;

			var processingResults = SendToProcessing(pack, isUsePA, _processingMethodName);

			Logger.LogInfo(string.Format("Запрос {0} к процессингу выполнен за {1}с", _tableName, (DateTime.Now - now).TotalSeconds.ToString("F1")), "");

			results = new PackResults();

			if (processingResults.IsSuccess)
			{
				results.IntegratePackResult = JsonConvert.DeserializeObject<List<PackResult>>(processingResults.ResponseStr);
				return true;
			}
			else
			{
				SetProcessingErrors(pack, processingResults.ResponseStr, processingResults.IsTimeout, isUsePA);
				return false;
			}
		}

		private RequestResult SendToProcessing(List<BaseIntegrationObject> pack, bool isUsePA, string methodName)
		{
			var processingIntegrationProvider = new ProcessingIntegrationProvider(isUsePA);
			return processingIntegrationProvider.Request(methodName, GetProcessingPackBody(pack));
		}

		private void SetProcessingErrors(List<BaseIntegrationObject> pack, string responseStr, bool isTimeout, bool isUsePA)
		{
			var errorMessage = String.IsNullOrEmpty(responseStr) ? String.Empty : responseStr.Replace("'", "''").Replace("{", "").Replace("}", "");
			if (errorMessage.Length > 250) errorMessage = errorMessage.Substring(0, 250);
			DBConnectionProvider.ExecuteNonQuery(String.Format("Update {1} Set {4} = {3}, ErrorMessage = '{2}' Where Id in ({0})", String.Join(",", pack.Select(p => String.Format("'{0}'", p.Id))), _tableName, errorMessage, isTimeout ? "4" : "2",
				isUsePA ? "StatusCS" : "StatusPC"));
		}


		protected virtual string GetSerializedCollection(List<BaseIntegrationObject> pack)
		{
			return String.Empty;
		}

		protected virtual string GetProcessingPackBody(List<BaseIntegrationObject> pack) { return String.Empty; }

		protected virtual void ProceedResults(PackResults results, bool isNeedSendToPersonalArea)
		{
			try
			{
				var columnName = isNeedSendToPersonalArea ? "StatusCS" : "StatusPC";
				var query = new StringBuilder();
				foreach (var result in results.IntegratePackResult)
				{
					if (result.IsSuccess)
					{
						query.AppendLine($"Update {_tableName} Set {columnName} = 1 Where Id = '{result.GetCorrectId()}';");
					}
					else
					{
						var errorMessage = String.IsNullOrEmpty(result.ErrorMessage) ? String.Empty : result.ErrorMessage.Replace("'", "''").Replace("{", "").Replace("}", "");
						if (errorMessage.Length > 250) errorMessage = errorMessage.Substring(0, 250);
						query.AppendLine($"Update {_tableName} Set {columnName} = 2, ErrorMessage = '{errorMessage}' Where Id = '{result.GetCorrectId()}';");
					}
				}

				var sql = query.ToString();
				if (!string.IsNullOrEmpty(sql))
					DBConnectionProvider.ExecuteNonQuery(sql);
			}
			catch (Exception e)
			{
				Logger.LogError(JsonConvert.SerializeObject(results), e);
			}
		}


		public virtual void ProceedResult(PackResult result, List<BaseIntegrationObject> pack, bool isNeedSendToPersonalArea)
		{
			lock (_lockRes)
			{
				try
				{
					var columnName = isNeedSendToPersonalArea ? "StatusCS" : "StatusPC";
					var query = new StringBuilder();

					if (result.IsSuccess)
					{
						foreach (var obj in pack)
						{
							query.AppendLine($"Update {_tableName} Set {columnName} = 1 Where Id = '{obj.Id}';");
						}
					}
					else
					{
						var errorMessage = String.IsNullOrEmpty(result.ErrorMessage) ? String.Empty : result.ErrorMessage.Replace("'", "").Replace("{", "").Replace("}", "");
						if (errorMessage.Length > 250) errorMessage = errorMessage.Substring(0, 250);
						foreach (var obj in pack)
						{
							query.AppendLine($"Update {_tableName} Set {columnName} = {2}, ErrorMessage = '{errorMessage}' Where Id = '{ obj.Id}';");
						}
					}

					var sql = query.ToString();
					if (!string.IsNullOrEmpty(sql))
						DBConnectionProvider.ExecuteNonQuery(sql);
				}
				catch (Exception e)
				{
					Logger.LogError(String.Format("Ошибка обновления состояний в ШТ {0} для первичного импорта", _tableName), e);
				}
			}
		}
	}
}
