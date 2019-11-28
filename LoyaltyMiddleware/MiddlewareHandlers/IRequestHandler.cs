using System.Collections.Generic;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	public interface IRequestHandler
	{
		Dictionary<string, object> GetHandledResponse(Dictionary<string, object> responseData);
	}
}
