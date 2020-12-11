using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedmondLoyaltyMiddleware.Integration;

namespace RedmondPersonalAreaIntegrationService.Controllers
{
	[Route("entity")]
	[ApiController]
	public class EntityController : ControllerBase
	{
		[HttpPost("LoadContactPack")]
		public ActionResult LoadContactPack([FromBody] IEnumerable<ContactProcessingModel> contacts)
		{
			return new ContactManager().LoadPack(contacts);
		}
	}
}
