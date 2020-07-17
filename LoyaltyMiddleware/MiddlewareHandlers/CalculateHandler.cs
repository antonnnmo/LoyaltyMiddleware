using LoyaltyMiddleware.DBProviders;
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
					var promocodesDictionary = new Dictionary<string, Guid>();
					var provider = new LoyaltyDBProvider();
					foreach (string promocode in promocodes)
					{
						if (!promocodesDictionary.ContainsKey(promocode))
						{
							promocodesDictionary[promocode] = provider.ExecuteSelectQuery<Guid>(@"SELECT ""ContactId"" FROM public.""PromoCode"" Where ""Code"" = '{0}'", reader => { return reader.GetGuid(0); }, promocode);
						}
					}


				}
			}

			return responseData;
		}

	}
}
