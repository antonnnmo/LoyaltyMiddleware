using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyMiddleware.Cache
{
	public class GlobalCacheReader
	{
		public static MemoryCache Cache { get; set; }

		static GlobalCacheReader()
		{
			Cache = new MemoryCache(new MemoryCacheOptions() { });
		}

		public static void LoadFromConfiguration(IConfiguration configuration)
		{
			Cache.Set(CacheKeys.ProcessingUri, configuration.GetValue<string>(CacheKeys.ProcessingUri));
			Cache.Set(CacheKeys.PersonalAreaUri, configuration.GetValue<string>(CacheKeys.PersonalAreaUri));
			Cache.Set(CacheKeys.ConnectionString, configuration.GetValue<string>(CacheKeys.ConnectionString));
			Cache.Set(CacheKeys.PersonalAreaConnectionString, configuration.GetValue<string>(CacheKeys.PersonalAreaConnectionString));
			Cache.Set(CacheKeys.BPMAuthCookieLifetimeMinutes, configuration.GetValue<string>(CacheKeys.BPMAuthCookieLifetimeMinutes));
			Cache.Set(CacheKeys.BPMLogin, configuration.GetSection("BPMCredentials").GetValue<string>("login"));
			Cache.Set(CacheKeys.BPMPassword, configuration.GetSection("BPMCredentials").GetValue<string>("password"));
			Cache.Set(CacheKeys.BPMUri, configuration.GetSection("BPMCredentials").GetValue<string>("uri"));
		}

		public static class CacheKeys
		{
			public static string BPMCookie { get { return "BPMCookie"; } }
			public static string BPMLogin { get { return "BPMLogin"; } }
			public static string BPMPassword { get { return "BPMPassword"; } }
			public static string BPMUri { get { return "BPMUri"; } }
			public static string BPMCSRF { get { return "BPMCSRF"; } }
			public static string ProcessingUri { get { return "ProcessingUri"; } }
			public static string ConnectionString { get { return "ConnectionString"; } }
			public static string PersonalAreaUri { get { return "PersonalAreaUri"; } }
			public static string PersonalAreaConnectionString { get { return "PersonalAreaConnectionString"; } }
			public static string BPMAuthCookieLifetimeMinutes { get { return "BPMAuthCookieLifetimeMinutes"; } }
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
