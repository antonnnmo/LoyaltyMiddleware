using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public static class GlobalCacheReader
	{
		public static MemoryCache Cache { get; set; }

		static GlobalCacheReader()
		{
			Cache = new MemoryCache(new MemoryCacheOptions() { });
		}

		public static class CacheKeys
		{
			public static string SqlConnectionString { get { return "SqlConnectionString"; } }
			public static string PackSize { get { return "PackSize"; } }
			public static string ThreadCount { get { return "ThreadCount"; } }
			public static string PersonalAreaUri { get { return "PersonalAreaUri"; } }
			public static string ProcessingUri { get { return "ProcessingUri"; } }
			public static string CrmRequestTimeout { get { return "CrmRequestTimeout"; } }
		}

		internal static void GetValue(object processingLogin, out string login)
		{
			throw new NotImplementedException();
		}

		public static bool GetValue<T>(string key, out T value)
		{
			return Cache.TryGetValue(key, out value);
		}

		public static void SetValue<T>(string key, T value)
		{
			GlobalCacheReader.Cache.Set(key, value);
		}

		public static void SetTemporaryValue<T>(string key, T value, TimeSpan lifeTime)
		{
			GlobalCacheReader.Cache.Set(key, value, lifeTime);
		}
	}
}
