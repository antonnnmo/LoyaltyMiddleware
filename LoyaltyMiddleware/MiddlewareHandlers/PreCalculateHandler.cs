using LoyaltyMiddleware.DBProviders;
using Newtonsoft.Json.Linq;
using RedmondLoyaltyMiddleware.Creatio;
using RedmondLoyaltyMiddleware.DBProviders;
using RedmondLoyaltyMiddleware.Models;
using RedmondLoyaltyMiddleware.Models.InternalDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondLoyaltyMiddleware.MiddlewareHandlers
{
	public class PreCalculateHandler : IPreRequestHandler
	{
		public PreHandlerResult GetHandledRequest(Dictionary<string, object> requestData, MiddlewareDBContext dbContext)
		{
			var result = new PreHandlerResult();

			if (Decimal.TryParse(dbContext.Setting.FirstOrDefault(s => s.Code == "MinProductPrice")?.Value, out decimal minPrice) && minPrice > 0)
			{
				if (requestData.ContainsKey("products"))
				{
					var products = requestData["products"] as Newtonsoft.Json.Linq.JArray;
					if (products != null && products.Count > 0)
					{
						foreach (JObject product in products) 
						{
							product.Add("minPrice", minPrice);
						}
					}
				}
			}

			if (requestData.ContainsKey("promoCodes"))
			{
				var promocodes = requestData["promoCodes"] as Newtonsoft.Json.Linq.JArray;
				if (promocodes != null && promocodes.Count > 0)
				{
					var contactId = GetContactId(requestData);
					if (contactId != Guid.Empty)
					{
						var promocodesDictionary = ReadPromocodesInformation(promocodes);
						var warnings = new List<string>();

						foreach (var promocodeInformation in promocodesDictionary)
						{
							if (promocodeInformation.Value != null)
							{
								var poolInfo = dbContext.PromocodePools.FirstOrDefault(pool => pool.Id == promocodeInformation.Value.PoolId);

								if (poolInfo == null)
								{
									promocodes.Remove(promocodes.FirstOrDefault(p => p.Value<string>() == promocodeInformation.Key));
									SavePromocodeAttempt(dbContext, promocodeInformation.Key, Guid.Empty, false, Guid.Empty, contactId);
									warnings.Add($"Промокод {promocodeInformation.Key} не найден");
								}
								else if (!poolInfo.IsActual) 
								{
									promocodes.Remove(promocodes.FirstOrDefault(p => p.Value<string>() == promocodeInformation.Key));
									SavePromocodeAttempt(dbContext, promocodeInformation.Key, poolInfo.Id, false, Constants.PromocodeStatuses.NotActual, contactId);
									warnings.Add($"Промокод {promocodeInformation.Key} более не актуален");
								}
								else if (promocodeInformation.Value.ContactId != Guid.Empty)
								{
									if (promocodeInformation.Value.ContactId == contactId)
									{
										CheckPromocodeLimit(dbContext, promocodes, warnings, promocodeInformation, poolInfo, contactId);
									}
									else 
									{
										promocodes.Remove(promocodes.FirstOrDefault(p => p.Value<string>() == promocodeInformation.Key));
										SavePromocodeAttempt(dbContext, promocodeInformation.Key, poolInfo.Id, false, Constants.PromocodeStatuses.UsesByAnotherContact, contactId);
										warnings.Add($"Промокод {promocodeInformation.Key} более не актуален");
									}
								}
								else if (promocodeInformation.Value.ContactId == Guid.Empty)
								{
									CheckPromocodeLimit(dbContext, promocodes, warnings, promocodeInformation, poolInfo, contactId);
								}
							}
							else 
							{
								promocodes.Remove(promocodes.FirstOrDefault(p => p.Value<string>() == promocodeInformation.Key));
								SavePromocodeAttempt(dbContext, promocodeInformation.Key, Guid.Empty, false, Guid.Empty, contactId);
								warnings.Add($"Промокод {promocodeInformation.Key} не найден");
							}
						}

						if (warnings.Count > 0) 
						{
							result.AdditionalResponseData.Add("promocodeWarnings", warnings);
						}
					}
				}
			}
			if (requestData.ContainsKey("deliveryForm")) 
			{
				requestData.Add("deliveryMethod", requestData["deliveryForm"]);
				requestData.Remove("deliveryForm");
			}
			if (requestData.ContainsKey("deliveryForm"))
			{
				requestData.Add("deliveryMethod", requestData["deliveryForm"]);
				requestData.Remove("deliveryForm");
			}
			if (requestData.ContainsKey("getMaxDiscount"))
			{
				requestData.Add("useMaxDiscount", requestData["getMaxDiscount"]);
				requestData.Remove("getMaxDiscount");
			}
			
			result.Request = requestData;
			return result;
		}

		private void CheckPromocodeLimit(MiddlewareDBContext dbContext, JArray promocodes, List<string> warnings, KeyValuePair<string, PromocodeInformation> promocodeInformation, PromocodePool poolInfo, Guid contactId)
		{
			var purchaseCount = GetPromocodeInPurchaseCount(promocodeInformation.Value.Id);
			if (purchaseCount >= poolInfo.UseCountRestriction)
			{
				promocodes.Remove(promocodes.FirstOrDefault(p => p.Value<string>() == promocodeInformation.Key));
				SavePromocodeAttempt(dbContext, promocodeInformation.Key, poolInfo.Id, false, Guid.Empty, contactId);
				warnings.Add($"Количество использований промокода {promocodeInformation.Key} исчерпано");
			}
			else
			{
				SavePromocodeAttempt(dbContext, promocodeInformation.Key, poolInfo.Id, true, Constants.PromocodeStatuses.Accepted, contactId);
			}
		}

		private Int64 GetPromocodeInPurchaseCount(Guid promocodeId)
		{
			var provider = new LoyaltyDBProvider();
			return provider.ExecuteScalar<Int64>(@"Select COUNT(*) from public.""PromoCodeInPurchase"" Where ""PromoCodeId"" = '{0}'", 0L, promocodeId.ToString());
		}

		private void SavePromocodeAttempt(MiddlewareDBContext dbContext, string promocode, Guid poolId, bool isNeedCreateNew, Guid statusId, Guid contactId) 
		{
			dbContext.PromocodeUseAttempts.Add(new PromocodeUseAttempt() {
				Date = DateTime.UtcNow,
				Id = Guid.NewGuid(),
				Promocode = promocode,
				PoolId = poolId,
				IsNeedCreateNew = isNeedCreateNew,
				StatusId = statusId,
				ContactId = contactId
			});

			dbContext.SaveChanges();
		}

		private Guid GetContactId(Dictionary<string, object> requestData)
		{
			if (requestData != null && requestData.ContainsKey("client") && requestData["client"] != null) 
			{
				var client = (requestData["client"] as JObject).ToObject<Dictionary<string, string>>();
				var mobilePhone = client.ContainsKey("mobilePhone") ? client["mobilePhone"] ?? String.Empty : String.Empty;
				var cardNumber = client.ContainsKey("cardNumber") ? client["cardNumber"] ?? String.Empty : String.Empty;
				var id = client.ContainsKey("id") ? client["id"] ?? Guid.Empty.ToString() : Guid.Empty.ToString();

				mobilePhone = mobilePhone.Replace("+", String.Empty);

				var provider = new LoyaltyDBProvider();
				var contactId = provider.ExecuteScalar<Guid>(@"Select ""Id"" from public.""Contact""
					Where
						(""Phone"" = '{0}' and '{0}' <> '')
					or
						(""Id"" = '{1}' and '{1}' <> '00000000-0000-0000-0000-000000000000')
					or
						('{2}' <> '' and (Select COUNT(*) from public.""Card"" Where ""ContactId"" = ""Contact"".""Id"" and ""Number"" = '{2}') > 0)",
						Guid.Empty, mobilePhone, id, cardNumber);
				return contactId;
			}

			return Guid.Empty;
		}

		private Dictionary<string, PromocodeInformation> ReadPromocodesInformation(Newtonsoft.Json.Linq.JArray promocodes)
		{
			var promocodesDictionary = new Dictionary<string, PromocodeInformation>();
			var provider = new LoyaltyDBProvider();
			foreach (string promocode in promocodes)
			{
				if (!promocodesDictionary.ContainsKey(promocode))
				{
					promocodesDictionary[promocode] = provider.ExecuteSelectQuery(@"SELECT ""ContactId"", ""IsUsed"", ""PoolId"", ""Id"" FROM public.""PromoCode"" Where ""Code"" = '{0}'", ReadPromocodeInformation, promocode);
				}
			}

			return promocodesDictionary;
		}

		public PromocodeInformation ReadPromocodeInformation(IDataReader reader)
		{
			PromocodeInformation result = null;
			if (reader.Read())
			{
				result = new PromocodeInformation()
				{
					ContactId = reader.GetValue("ContactId", Guid.Empty),
					Id = reader.GetValue("Id", Guid.Empty),
					IsUsed = reader.GetValue("IsUsed", false),
					PoolId = reader.GetValue("PoolId", Guid.Empty),
				};
			}

			reader.Close();
			reader.Dispose();

			return result;
		}
	}
}
