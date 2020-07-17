using System.Collections.Generic;
using LoyaltyMiddleware.Loyalty;
using LoyaltyMiddleware.MiddlewareHandlers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedmondLoyaltyMiddleware.Models.InternalDB;

namespace LoyaltyMiddleware.Controllers
{
	[Route("purchase")]
	[ApiController]
	public class PurchaseController : ControllerBase
	{
		private MiddlewareDBContext _dbContext;

		public PurchaseController(MiddlewareDBContext context)
		{
			_dbContext = context;
		}

		[HttpPost("calculate")]
		public ActionResult Calculate([FromBody] Dictionary<string, object> request)
		{
			return HandleRequest(request, "calculate", new CalculateHandler());
		}

		[HttpPost("confirm")]
		public ActionResult Confirm([FromBody] Dictionary<string, object> request)
		{
			return HandleRequest(request, "confirm", new ConfirmHandler());
		}

		private ActionResult HandleRequest(Dictionary<string, object> request, string method, IRequestHandler handler)
		{
			var authHeader = HttpContext.Request.Headers["Authorization"];
			if (authHeader.Count == 0) return Unauthorized();

			Logger.LogInfo($"started {method} request", "");

			var response = new ProcessingManager().PRRequest($"purchase/{method}", JsonConvert.SerializeObject(request), authHeader);

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
