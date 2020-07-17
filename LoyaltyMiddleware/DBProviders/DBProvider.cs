using LoyaltyMiddleware.Cache;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyMiddleware.DBProviders
{
	public class LoyaltyDBProvider
	{
		public virtual string GetConnectionString()
		{
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.ProcessingConnectionString, out string connString);
			return connString;
		}

		public T ExecuteSelectQuery<T>(string command, Func<NpgsqlDataReader, T> readerMethod, params object[] parameters)
		{
			using (var conn = new NpgsqlConnection(GetConnectionString()))
			{
				conn.Open();

				// Insert some data
				using (var cmd = new NpgsqlCommand(ValidateCommand(command, parameters), conn))
				{
					using (var reader = cmd.ExecuteReader())
					{
						return readerMethod(reader);
					}
				}
			}
		}

		public T ExecuteScalar<T>(string command, T defValue, params string[] parameters)
		{
			using (var conn = new NpgsqlConnection(GetConnectionString()))
			{
				conn.Open();

				// Insert some data
				using (var cmd = new NpgsqlCommand(ValidateCommand(command, parameters), conn))
				{
					var res = cmd.ExecuteScalar();
					if (res == null || res is DBNull) return defValue;
					return (T)res;
				}
			}
		}

		public int ExecuteNonQuery(string command, params string[] parameters)
		{
			using (var conn = new NpgsqlConnection(GetConnectionString()))
			{
				conn.Open();
				using (var sqlCommand = new NpgsqlCommand(ValidateCommand(command, parameters), conn))
				{
					return sqlCommand.ExecuteNonQuery();
				}
			}
		}

		private string ValidateCommand(string command, object[] parameters)
		{
			if (parameters == null) return command;
			return String.Format(command, parameters.Select(p => p == null || String.IsNullOrEmpty(p.ToString()) ? String.Empty : p.ToString().Replace("'", "''")).ToArray());
		}
	}
}
