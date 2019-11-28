using System.Collections.Generic;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	internal class ConfirmHandler: IRequestHandler
	{
		public ConfirmHandler()
		{

		}

		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> responseData)
		{
			//todo: confirm middlewarecode

			return responseData;
		}
	}
}