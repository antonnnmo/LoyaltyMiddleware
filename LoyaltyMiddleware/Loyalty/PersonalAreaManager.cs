using LoyaltyMiddleware.Cache;

namespace LoyaltyMiddleware.Loyalty
{
	public class PersonalAreaManager: ProcessingManager
	{
		public PersonalAreaManager()
		{
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.PersonalAreaUri, out _uri);
		}
	}
}
