using System;
using System.Collections.Generic;

namespace RedmondLoyaltyMiddleware.Creatio
{
	public class PromocodeUseAttemptServiceResponse
	{
		public PromocodeUseAttemptResult AddPromocodeUseAttemptResult { get; set; }
		public class ErrorPromocodeDto 
		{
			public Guid Key { get; set; }
			public string Value { get; set; }
		}
		public class PromocodeUseAttemptResult : BaseCRMServiceResponse
		{
			public List<ErrorPromocodeDto> ErrorPromocodes { get; set; }
		}
	}
}
