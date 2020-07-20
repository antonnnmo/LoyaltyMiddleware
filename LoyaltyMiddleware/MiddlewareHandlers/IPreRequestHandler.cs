using RedmondLoyaltyMiddleware.Models.InternalDB;
using System.Collections.Generic;

namespace RedmondLoyaltyMiddleware.MiddlewareHandlers
{
	interface IPreRequestHandler
	{
		PreHandlerResult GetHandledRequest(Dictionary<string, object> requestData, MiddlewareDBContext dbContext);
	}
}
