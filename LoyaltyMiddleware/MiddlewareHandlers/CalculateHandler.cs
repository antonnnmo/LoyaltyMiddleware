using System.Collections.Generic;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	public class CalculateHandler : IRequestHandler
	{
		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> responseData)
		{
			//todo: calculate middlewarecode

			return responseData;
		}
	}
}
