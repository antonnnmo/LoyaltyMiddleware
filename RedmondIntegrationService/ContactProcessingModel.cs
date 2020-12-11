using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class ContactProcessingModel : BaseProcessingModel
	{
		public string Name { get; set; }
		public string Phone { get; set; }

		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string MiddleName { get; set; }
		public string Email { get; set; }
		public string Gender { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public string Birthday { get; set; }
		public string ShopCode { get; set; }
		public string PhoneConfirmed { get; set; }
		public string RegistrationDate { get; set; }
		public string Code { get; set; }
	}
}
