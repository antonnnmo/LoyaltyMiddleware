using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyMiddleware.Loyalty;
using LoyaltyMiddleware.MiddlewareHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LoyaltyMiddleware.Controllers
{
	[Route("contact")]
	[ApiController]
	public class PersonalAreaController : ControllerBase
	{
		[HttpPost("info")]
		public ActionResult Info([FromBody] Dictionary<string, object> request)
		{
			return HandleRequest(request, "info", new InfoHandler());
		}

		private ActionResult HandleRequest(Dictionary<string, object> request, string method, IRequestHandler handler)
		{
			var authHeader = HttpContext.Request.Headers["Authorization"];
			if (authHeader.Count == 0) return Unauthorized();

			Logger.LogInfo($"started {method} request", "");

			var response = new PersonalAreaManager().PRRequest($"purchase/{method}", JsonConvert.SerializeObject(request), authHeader);

			Logger.LogInfo($"finished {method} request", "");


			if (response.IsSuccess)
			{
				HttpContext.Response.Headers.Add("Content-Type", "application/json");
				var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.ResponseStr);
				var handledResponse = handler.GetHandledResponse(request, responseData);
				return Ok(handledResponse);
			}
			else
			{
				return BadRequest(response.ResponseStr);
			}
		}
	}
}