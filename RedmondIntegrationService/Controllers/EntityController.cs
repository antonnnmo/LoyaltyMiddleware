using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RedmondIntegrationService.Controllers
{
	[Route("api/Entity")]
	[ApiController]
	public class EntityController : ControllerBase
	{
		[HttpGet("StartIntegratePC")]
		public ActionResult StartIntegratePC() 
		{
			new Task(() => { new ContactManager().Execute(false); }).Start();

			return Ok();
		}

		[HttpGet("StartIntegrateCS")]
		public ActionResult StartIntegrateCS()
		{
			new Task(() => { new ContactManager().Execute(true); }).Start();

			return Ok();
		}
	}
}
