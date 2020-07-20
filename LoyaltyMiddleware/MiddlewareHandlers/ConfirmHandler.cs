using RedmondLoyaltyMiddleware.Models.InternalDB;
using System.Collections.Generic;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	internal class ConfirmHandler : IRequestHandler
	{
		public ConfirmHandler()
		{

		}

		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> requestData, Dictionary<string, object> responseData, Dictionary<string, object> additionalResponseData, MiddlewareDBContext dbContext)
		{
			//todo: confirm middlewarecode

			return responseData;
		}
	}
}