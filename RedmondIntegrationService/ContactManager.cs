using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class ContactManager : BaseManager
	{
		private static readonly string _selectContact = @"SELECT TOP ({0}) 
				Id
				,[Surname]
				,[GivenName]
				,[MiddleName]
				,MobilePhone
				,Email
				,GenderCode
				,CONVERT(nvarchar(50), BirthDate, 21) as BirthDate
				,CONVERT(nvarchar(50), RegistrationDate, 21) as RegistrationDate
				,[RegistrationPlaceCode]
				,[Code]
		  FROM [Contact]
		 Where {1} = 0";

		protected List<Contact> _contacts;

		public ContactManager()
		{
			_tableName = "Contact";
			_processingPrimaryMethodName = "LoadPrimaryContactPack";
			_processingMethodName = "LoadContactPack";
			_isNeedSendToProcessing = true;
			_isNeedSendToPersonalArea = true;
		}

		protected override List<BaseIntegrationObject> ReadPack(bool isPC)
		{
			var columnName = isPC ? "StatusPC" : "StatusCS";
			var pack = new List<BaseIntegrationObject>();
			lock (_lock)
			{
				using (var provider = new DBConnectionProvider())
				{
					using (var reader = provider.Execute(_selectContact, packSize, columnName))
					{
						while (reader != null && reader.Read())
						{
							pack.Add(new Contact()
							{
								Email = reader.GetValue("email", String.Empty),
								FirstName = reader.GetValue("GivenName", String.Empty),
								MiddleName = reader.GetValue("MiddleName", String.Empty),
								Phone = reader.GetValue("MobilePhone", String.Empty),
								RegistrationDate = reader.GetValue("RegistrationDate", String.Empty),
								BirthDay = reader.GetValue("BirthDate", String.Empty),
								Id = reader.GetValue("Id", Guid.Empty),
								Surname = reader.GetValue("Surname", String.Empty),
								Code = reader.GetValue("Code", String.Empty),
								ShopCode = reader.GetValue("RegistrationPlaceCode", String.Empty),
								IsGenderNull = String.IsNullOrEmpty(reader.GetValue("GenderCode", String.Empty)),
								IsMan = reader.GetValue("GenderCode", String.Empty).ToLower() == "male",
							});
						}
					}
				}

				if (pack.Count > 0)
				{
					DBConnectionProvider.ExecuteNonQuery($"Update Contact Set {columnName} = 3 Where Id in ({String.Join(",", pack.Select(p => String.Format("'{0}'", p.Id)))})");
				}
			}

			return pack;
		}

		protected override string GetSerializedCollection(List<BaseIntegrationObject> pack)
		{
			return JsonConvert.SerializeObject(pack.Select(p => (Contact)p).ToList());
		}

		protected override string GetProcessingPackBody(List<BaseIntegrationObject> pack)
		{
			var contacts = pack.Select(c => (Contact)c).Select(c => new ContactProcessingModel()
			{
				Id = c.Id,
				Name = GetContactName(c.Surname, c.FirstName, c.MiddleName),
				Phone = c.Phone,
				Address = c.Address,
				Birthday = c.BirthDay,
				City = c.City,
				Country = c.Country,
				Email = c.Email,
				FirstName = c.FirstName,
				Gender = c.IsMan ? "1" : "0",
				MiddleName = c.MiddleName,
				RegistrationDate = c.RegistrationDate,
				ShopCode = c.ShopCode,
				Surname = c.Surname,
			});
			return JsonConvert.SerializeObject(contacts);
		}

		private string GetContactName(string surname, string firstName, string middleName)
		{
			var names = new List<string> { surname, firstName, middleName };
			return string.Join(" ", names);
		}
	}
}
