using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class BaseIntegrationObject
	{
        [JsonProperty]
        public Guid Id { get; set; }
    }
}
