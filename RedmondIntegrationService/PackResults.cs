using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedmondIntegrationService
{
	public class PackResults
	{
		[JsonProperty]
		public List<PackResult> IntegratePackResult { get; set; }
	}

	public class PrimaryIntegratePackResponse
	{
		[JsonProperty]
		public List<PackResult> PrimaryIntegratePackResult { get; set; }
	}

	public class PackResult
	{
		[JsonProperty]
		public bool IsTimeout { get; set; }
		[JsonProperty]
		public string Id { get; set; }
		[JsonProperty]
		public bool IsSuccess { get; set; }
		[JsonProperty]
		public string ErrorMessage { get; set; }
		[JsonProperty]
		public string ContactId { get; set; }

		public string GetCorrectId()
		{
			return (Id ?? "").Replace("'", "''");
		}
	}
}
