using RedmondLoyaltyMiddleware.Models.InternalDB;
using System.Collections.Generic;
using System.Linq;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	public class CalculateHandler : IRequestHandler
	{
		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> requestData, Dictionary<string, object> responseData, Dictionary<string, object> additionalResponseData, MiddlewareDBContext dbContext)
		{
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
