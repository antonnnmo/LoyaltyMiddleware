using System;

namespace RedmondLoyaltyMiddleware.Models.InternalDB
{
	public class PromocodeUseAttempt
	{
		public Guid Id { get; set; }
		public string Promocode { get; set; }
		public DateTime Date { get; set; }
		public Guid PoolId { get; set; }
		public Guid StatusId { get; set; }
		public bool IsNeedCreateNew { get; set; }
		public string Error { get; set; }
		public bool IsError { get; set; }
		public Guid ContactId { get; set; }
	}
}
