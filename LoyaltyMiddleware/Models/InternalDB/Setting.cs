using System;

namespace RedmondLoyaltyMiddleware.Models.InternalDB
{
	public class Setting
	{
		public Guid Id { get; set; }
		public string Code { get; set; }
		public string Value { get; set; }
	}
}
