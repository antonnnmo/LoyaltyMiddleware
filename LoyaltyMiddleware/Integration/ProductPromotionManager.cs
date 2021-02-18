using LoyaltyMiddleware.DBProviders;
using Microsoft.AspNetCore.Mvc;
using RedmondLoyaltyMiddleware.DBProviders;
using RedmondLoyaltyMiddleware.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondLoyaltyMiddleware.Integration
{
	public class ProductPromotionManager : ControllerBase
	{
		public ActionResult QueryProductSegment(string productId)
		{
			var provider = new LoyaltyDBProvider();
			var query = @"
				with ProductPromotion as 
				(
					select ps.""Id"" as ""SegmentId"" , ps.""Name"" as ""SegmentName"", p2.""Id"" as ""PromotionId"", p2.""Name"" as ""PromotionName""
					from ""ProductInSegment"" pis 
					inner join ""ProductSegment"" ps on ps.""Id"" = pis.""SegmentId"" 
					inner join ""PurchaseBenefit"" pb on pb.""Discount_SegmentId"" = ps.""Id"" 
					or pb.""FixedPrice_SegmentId"" = ps.""Id"" 
					or pb.""ChargeBonus_PercentCharge_ProductSegmentId"" = ps.""Id"" 
					or pb.""AllowBonusPaymentParams_ProductSegmentId"" = ps.""Id"" 
					inner join ""Promotion"" p2 on p2.""Id"" = pb.""PromotionId""
					where pis.""ProductId"" = '{0}'
					union
					select ps.""Id"" as ""SegmentId"" , ps.""Name"" as ""SegmentName"", p2.""Id"" as ""PromotionId"", p2.""Name"" as ""PromotionName""
					from ""ProductInSegment"" pis 
					inner join ""ProductSegment"" ps on ps.""Id"" = pis.""SegmentId"" 
					inner join ""BenefitCondition"" bc on bc.""SegmentId"" = ps.""Id""
					inner join  ""PurchaseBenefit"" pb on bc.""PurchaseBenefitId"" = pb.""Id""
					inner join ""Promotion"" p2 on p2.""Id"" = pb.""PromotionId""
					where pis.""ProductId"" = '{0}'
				)
				select distinct ""SegmentId"", ""SegmentName"", ""PromotionId"", ""PromotionName"" from ProductPromotion
			";
			var items = provider.ExecuteSelectQuery(query, ReadProductSegment, productId);
			return Ok(items);
		}


		public List<ProductPromotion> ReadProductSegment(IDataReader reader)
		{
			var result = new List<ProductPromotion>();
			while (reader.Read())
			{
				result.Add(new ProductPromotion()
				{
					SegmentId = reader.GetValue("SegmentId", Guid.Empty),
					SegmentName = reader.GetValue("SegmentName", String.Empty),
					PromotionId = reader.GetValue("PromotionId", Guid.Empty),
					PromotionName = reader.GetValue("PromotionName", String.Empty)
				});
			}

			reader.Close();
			reader.Dispose();

			return result;
		}

	}

}
