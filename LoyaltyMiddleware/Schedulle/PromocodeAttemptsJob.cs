using FluentScheduler;
using LoyaltyMiddleware;
using LoyaltyMiddleware.Cache;
using LoyaltyMiddleware.Creatio;
using Newtonsoft.Json;
using RedmondLoyaltyMiddleware.Creatio;
using RedmondLoyaltyMiddleware.Models.InternalDB;
using System.Linq;

namespace RedmondLoyaltyMiddleware.Schedulle
{
	public class PromocodeAttemptsJob : IJob
	{
		private readonly object _lock = new object();

		private bool _shuttingDown;
		private readonly MiddlewareDBContext _dBContext;

		public PromocodeAttemptsJob(MiddlewareDBContext dbContext)
		{
			_dBContext = dbContext;
		}

		public void Execute()
		{
			try
			{
				lock (_lock)
				{
					if (_shuttingDown)
						return;

					GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.PromocodeAttemptsTransferPackSize, out int packSize);

					var crmProvider = new CRMIntegrationProvider(true);
					while (_dBContext.PromocodeUseAttempts.Where(p => !p.IsError).Count() > 0) 
					{
						var pack = _dBContext.PromocodeUseAttempts.Where(p => !p.IsError).Take(packSize).ToList();
						var microsoftDateFormatSettings = new JsonSerializerSettings
						{
							DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
						};
						var result = crmProvider.MakeRequest("PromocodeService\\AddPromocodeUseAttempt", $"{{\"promocodeUseAttempts\": {JsonConvert.SerializeObject(pack, microsoftDateFormatSettings)}}}" );
						if (result == null || !result.IsSuccess)
						{
							Logger.LogError($"Ошибка вызова сервиса PromocodeService\\AddPromocodeUseAttempt: {result?.ResponseStr}");
						}
						else 
						{
							var response = JsonConvert.DeserializeObject<PromocodeUseAttemptServiceResponse>(result.ResponseStr).AddPromocodeUseAttemptResult;

							if (!response.IsSuccess)
							{
								Logger.LogError($"Ошибка вызова сервиса PromocodeService\\AddPromocodeUseAttempt: {response.ErrorMessage}");
							}
							else 
							{
								var errorPromocodes = response.ErrorPromocodes.ToDictionary(pair => pair.Key, pair => pair.Value);
								var successWritedPromocodes = pack.Where(p => !errorPromocodes.ContainsKey(p.Id));
								if(successWritedPromocodes.Count() > 0) _dBContext.RemoveRange(successWritedPromocodes);

								foreach (var errorPromocode in errorPromocodes) 
								{
									var promocode = pack.FirstOrDefault(p => p.Id == errorPromocode.Key);
									if (promocode != null) 
									{
										promocode.Error = errorPromocode.Value;
										promocode.IsError = true;
										_dBContext.Update(promocode);
									}
								}
								_dBContext.SaveChanges();
							}
						}
					}
				}
			}
			finally
			{
			}
		}

		public void Stop(bool immediate)
		{
			lock (_lock)
			{
				_shuttingDown = true;
			}

		}
	}
}
