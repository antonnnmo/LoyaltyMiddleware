using LoyaltyMiddleware.DBProviders;
using RedmondLoyaltyMiddleware.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	public class CalculateHandler : IRequestHandler
	{
		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> requestData, Dictionary<string, object> responseData)
		{
			//todo: calculate middlewarecode
			if (requestData.ContainsKey("promoCodes"))
			{
				var promocodes = requestData["promoCodes"] as Newtonsoft.Json.Linq.JArray;
				if (promocodes.Count > 0)
				{
					var promocodesDictionary = new Dictionary<string, PromocodeInformation>();
					var provider = new LoyaltyDBProvider();
					/*foreach (string promocode in promocodes)
					{
						if (!promocodesDictionary.ContainsKey(promocode))
						{
							promocodesDictionary[promocode] = provider.ExecuteSelectQuery(
								@"SELECT 
									pool.""CanUseManyTimes"", 
									promocode.""ContactId"", 
									promocode.""IsUsed"", 
									pool.""IsActual"", 
									pool.""UseCountRestriction"" 
								FROM public.""PromoCode"" promocode
								join public.""PromoCodePool"" pool on pool.""Id"" = promocode.""PoolId"" 
								Where promocode.""Code"" = '{0}'", ReadPromocodeInformation, promocode);
						}
					}*/
				}
			}
			return responseData;
		}

		public PromocodeInformation ReadPromocodeInformation(IDataReader reader) 
		{
			return new PromocodeInformation();
		}
	}
}
