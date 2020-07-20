using FluentScheduler;
using LoyaltyMiddleware.Cache;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RedmondLoyaltyMiddleware.Schedulle
{
	public class Scheduller: Registry
	{
		public Scheduller(IServiceProvider sp)
		{
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.PromocodeAttemptsTransferPeriodInMinutes, out int period);
			Schedule(() => sp.CreateScope().ServiceProvider.GetRequiredService<PromocodeAttemptsJob>()).ToRunNow().AndEvery(period).Minutes();
		}
	}
}
