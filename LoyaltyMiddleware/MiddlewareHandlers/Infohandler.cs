using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyMiddleware.MiddlewareHandlers
{
	public class InfoHandler : IRequestHandler
	{
		public Dictionary<string, object> GetHandledResponse(Dictionary<string, object> requestData, Dictionary<string, object> responseData)
		{
			//todo: confirm middlewarecode

			return responseData;
		}
	}
}
