using System;

namespace RedmondLoyaltyMiddleware.Models.InternalDB
{
	public class PromocodePool
	{
		public Guid Id { get; set; }
		public bool CanUseManyTimes { get; set; }
		public bool IsActual { get; set; }
		public int UseCountRestriction { get; set; }
	}
}
