using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using LoyaltyMiddleware;
using LoyaltyMiddleware.Loyalty;
using LoyaltyMiddleware.MiddlewareHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RedmondLoyaltyMiddleware.Integration;
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
				switch (request.GetValueOrDefault("entityName"))
				{
					case "PromocodePool":
						{
							if (request.ContainsKey("entity"))
							{
								var pool = JsonConvert.DeserializeObject<PromocodePool>((request.GetValueOrDefault("entity")).ToString());

								if (!_dbContext.PromocodePools.Any(x => x.Id == pool.Id))
								{
									_dbContext.Add(pool);
									_dbContext.SaveChanges();
								}
								else
								{
									_dbContext.Update(pool);
									_dbContext.SaveChanges();
								}

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
				switch (request.GetValueOrDefault("entityName"))
				{
					case "PromocodePool":
						{
							if (request.ContainsKey("entity"))
							{
								var pool = JsonConvert.DeserializeObject<PromocodePool>((request.GetValueOrDefault("entity")).ToString());

								if (_dbContext.PromocodePools.Any(x=>x.Id == pool.Id))
								{
									_dbContext.Update(pool);
									_dbContext.SaveChanges();
								}
								else
								{
									_dbContext.Add(pool);
									_dbContext.SaveChanges();
								}							

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
				switch (request.GetValueOrDefault("entityName"))
				{
					case "PromocodePool":
						{
							if (request.ContainsKey("entity"))
							{
								var pool = JsonConvert.DeserializeObject<PromocodePool>((request.GetValueOrDefault("entity")).ToString());

								if (_dbContext.PromocodePools.Any(x => x.Id == pool.Id))
								{
									_dbContext.Remove(pool);
									_dbContext.SaveChanges();
								}
									
								return Ok();
							}
						}
						break;
				}
			}
			return BadRequest();
		}

		[HttpPost("LoadContactPack")]
		public ActionResult LoadContactPack([FromBody] IEnumerable<ContactProcessingModel> contacts)
		{
			return new ContactManager().LoadPack(contacts);
		}
	}
}