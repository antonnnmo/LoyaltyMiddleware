using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondLoyaltyMiddleware.Models
{
	public class PromocodeInformation
	{
		public Guid ContactId { get; internal set; }
		public bool IsUsed { get; internal set; }
		public Guid PoolId { get; internal set; }
		public Guid Id { get; internal set; }
	}
}
