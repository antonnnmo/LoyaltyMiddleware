using LoyaltyMiddleware.Cache;

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
