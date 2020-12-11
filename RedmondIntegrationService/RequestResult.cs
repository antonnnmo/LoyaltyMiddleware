using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class RequestResult
	{
		public bool IsSuccess { get; set; }
		//public bool IsUnathorized { get; set; }
		public bool IsTimeout { get; set; }
		public string ResponseStr { get; set; }
	}
}
