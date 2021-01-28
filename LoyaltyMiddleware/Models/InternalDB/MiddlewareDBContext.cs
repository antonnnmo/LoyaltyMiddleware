using LoyaltyMiddleware.Cache;
using Microsoft.EntityFrameworkCore;

namespace RedmondLoyaltyMiddleware.Models.InternalDB
{
	public class MiddlewareDBContext : DbContext
	{
		public MiddlewareDBContext() : base()
		{

		}
		public MiddlewareDBContext(DbContextOptions<MiddlewareDBContext> options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.ConnectionString, out string connectionString);
			optionsBuilder.UseNpgsql(connectionString);
		}

		public DbSet<PromocodeUseAttempt> PromocodeUseAttempts { get; set; }
		public DbSet<PromocodePool> PromocodePools { get; set; }
		public DbSet<Setting> Setting { get; set; }
	}
}
