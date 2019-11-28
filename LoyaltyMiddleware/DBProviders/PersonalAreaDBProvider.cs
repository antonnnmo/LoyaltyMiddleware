using LoyaltyMiddleware.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyMiddleware.DBProviders
{
	public class PersonalAreaDBProvider: LoyaltyDBProvider
	{
		public override string GetConnectionString()
		{
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.PersonalAreaConnectionString, out string connString);
			return connString;
		}
	}
}
