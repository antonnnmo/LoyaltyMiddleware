using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondLoyaltyMiddleware.Creatio
{
	public class BaseCRMServiceResponse
	{
		public bool IsSuccess { get; set; }
		public int ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
	}
}
