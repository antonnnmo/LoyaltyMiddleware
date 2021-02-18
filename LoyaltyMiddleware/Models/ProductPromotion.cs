using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondLoyaltyMiddleware.Models
{
	public class ProductPromotion
	{
		public Guid SegmentId { get; internal set; }
		public string SegmentName { get; internal set; }
		public Guid PromotionId { get; internal set; }
		public string PromotionName { get; internal set; }
	}
}
