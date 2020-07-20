using LoyaltyMiddleware.Cache;

namespace LoyaltyMiddleware.DBProviders
{
	public class MiddlewareDBProvider : LoyaltyDBProvider
	{
		public override string GetConnectionString()
		{
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.ConnectionString, out string connString);
			return connString;
		}
	}
}
