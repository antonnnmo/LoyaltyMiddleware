using System.Collections.Generic;
using LoyaltyMiddleware;
using LoyaltyMiddleware.Loyalty;
using LoyaltyMiddleware.MiddlewareHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedmondLoyaltyMiddleware.Models.InternalDB;

namespace RedmondLoyaltyMiddleware.Controllers
{
	[Route("entity")]
	[ApiController]
	public class EntityController : ControllerBase
	{
		private MiddlewareDBContext _dbContext;

		public EntityController(MiddlewareDBContext context)
		{
			_dbContext = context;
		}

		[HttpPost("insert")]
		public ActionResult Insert([FromBody] Dictionary<string, object> request)
		{
			if (request.ContainsKey("entityName"))
			{
				//switch (((System.Text.Json.JsonElement)request.GetValueOrDefault("entityName")).GetString())
				switch (request.GetValueOrDefault("entityName"))
				{
					case "PromocodePool":
						{
							if (request.ContainsKey("entity"))
							{
								//var pool = JsonConvert.DeserializeObject<PromocodePool>(((System.Text.Json.JsonElement)request.GetValueOrDefault("entity")).ToString());
								var pool = JsonConvert.DeserializeObject<PromocodePool>((request.GetValueOrDefault("entity")).ToString());

								var provider = new LoyaltyMiddleware.DBProviders.MiddlewareDBProvider();
								provider.ExecuteNonQuery(
									@"Insert into public.""PromocodePools""
										(""Id"", ""CanUseManyTimes"", ""IsActual"", ""UseCountRestriction"")
										Values ('{0}', '{1}', '{2}', '{3}')",
									pool.Id.ToString(), pool.CanUseManyTimes.ToString(), pool.IsActual.ToString(), pool.UseCountRestriction.ToString());
								return Ok();
							}
						}
						break;
				}
			}
			return BadRequest();
		}

		[HttpPost("update")]
		public ActionResult Update([FromBody] Dictionary<string, object> request)
		{
			if (request.ContainsKey("entityName"))
			{
				//switch (((System.Text.Json.JsonElement)request.GetValueOrDefault("entityName")).GetString())
				switch (request.GetValueOrDefault("entityName"))
				{
					case "PromocodePool":
						{
							if (request.ContainsKey("entity"))
							{
								//var pool = JsonConvert.DeserializeObject<PromocodePool>(((System.Text.Json.JsonElement)request.GetValueOrDefault("entity")).ToString());
								var pool = JsonConvert.DeserializeObject<PromocodePool>((request.GetValueOrDefault("entity")).ToString());

								var provider = new LoyaltyMiddleware.DBProviders.MiddlewareDBProvider();
								provider.ExecuteNonQuery(
									@"Update public.""PromocodePools""
										set ""CanUseManyTimes"" = '{1}',
											""IsActual"" = '{2}',
											""UseCountRestriction"" = '{3}'
										where ""Id"" = '{0}'",
									pool.Id.ToString(), pool.CanUseManyTimes.ToString(), pool.IsActual.ToString(), pool.UseCountRestriction.ToString());
								return Ok();
							}
						}
						break;
				}
			}
			return BadRequest();
		}

		[HttpPost("delete")]
		public ActionResult Delete([FromBody] Dictionary<string, object> request)
		{
			if (request.ContainsKey("entityName"))
			{
				//switch (((System.Text.Json.JsonElement)request.GetValueOrDefault("entityName")).GetString())
				switch (request.GetValueOrDefault("entityName"))
				{
					case "PromocodePool":
						{
							if (request.ContainsKey("entity"))
							{
								//var pool = JsonConvert.DeserializeObject<PromocodePool>(((System.Text.Json.JsonElement)request.GetValueOrDefault("entity")).ToString());
								var pool = JsonConvert.DeserializeObject<PromocodePool>((request.GetValueOrDefault("entity")).ToString());

								var provider = new LoyaltyMiddleware.DBProviders.MiddlewareDBProvider();
								provider.ExecuteNonQuery(
									@"Delete from public.""PromocodePools""
										where ""Id"" = '{0}'",
									pool.Id.ToString());
								return Ok();
							}
						}
						break;
				}
			}
			return BadRequest();

			
		}

	}
}