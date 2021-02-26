using RedmondLoyaltyMiddleware.Models.InternalDB;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	public class CalculateHandler : IRequestHandler
	{
		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> requestData, Dictionary<string, object> responseData, Dictionary<string, object> additionalResponseData, MiddlewareDBContext dbContext)
		{
			if (responseData.ContainsKey("data"))
			{
				JObject data = responseData["data"] as JObject;
				if (data.ContainsKey("productDiscounts"))
				{
					JArray productDiscounts = data["productDiscounts"] as JArray;
					if (productDiscounts != null && productDiscounts.Count > 0)
					{
						foreach (JObject productDiscount in productDiscounts)
						{
							var discountAmount = 0;
							JArray discounts = productDiscount["discounts"] as JArray;
							if (discounts != null && discounts.Count > 0)
							{
								foreach (JObject discount in discounts)
									if (discount.ContainsKey("discount"))
									{
										var d = Convert.ToInt32(Math.Round((decimal)discount["discount"]));
										discount["discount"] = d;
										discountAmount += d;
									}
							}
							if (productDiscount.ContainsKey("discount"))
								productDiscount["discount"] = discountAmount;
						}
					}
				}
			}


			if (additionalResponseData != null) 
			{
				responseData = new Dictionary<string, object>[] { responseData, additionalResponseData }.SelectMany(dict => dict)
						 .ToDictionary(pair => pair.Key, pair => pair.Value);
			}
			return responseData;
		}

		/*private Guid GetContactId(Dictionary<string, object> responseData)
		{
			var data = responseData["data"] as JObject;
			var client = data.ToObject<Dictionary<string, object>>()["client"] as JObject;
			return new Guid(client.ToObject<Dictionary<string, object>>()["id"].ToString());
		}*/
	}
}
