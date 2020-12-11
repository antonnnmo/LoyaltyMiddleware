using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class Contact : BaseIntegrationObject
	{
		[JsonProperty]
		public string BirthDay { get; set; }
		[JsonProperty]
		public string ErrorMessage { get; set; }
		[JsonProperty]
		public string CreatedOn { get; set; }
		[JsonProperty]
		public string FirstName { get; set; }
		[JsonProperty]
		public string Surname { get; set; }
		[JsonProperty]
		public string MiddleName { get; set; }
		[JsonProperty]
		public string Email { get; set; }
		[JsonProperty]
		public string Phone { get; set; }
		[JsonProperty]
		public bool IsMan { get; set; }
		[JsonProperty]
		public string Country { get; set; }
		[JsonProperty]
		public string City { get; set; }
		[JsonProperty]
		public string Address { get; set; }
		[JsonProperty]
		public string RegistrationDate { get; set; }
		[JsonProperty]
		public string ShopCode { get; set; }
		[JsonProperty]
		public string ContactStatus { get; set; }
		[JsonProperty]
		public int Type { get; set; }
		[JsonProperty]
		public string Code { get; set; }
		[JsonProperty]
		public bool IsGenderNull { get; set; }

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		public static Contact FromJson(string json) => JsonConvert.DeserializeObject<Contact>(json);
	}
}
