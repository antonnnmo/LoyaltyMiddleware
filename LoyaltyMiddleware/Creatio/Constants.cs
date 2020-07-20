using System;

namespace RedmondLoyaltyMiddleware.Creatio
{
	public static class Constants
	{
		public static class PromocodeStatuses
		{
			public static readonly Guid NotActual = new Guid("C6375327-37E6-433D-B320-24325CCF71A3");
			public static readonly Guid UsesByAnotherContact = new Guid("3F55C64E-5989-42B3-A672-3D37190AFB75");
			public static readonly Guid Accepted = new Guid("40CF643E-04D4-49FC-A0D5-9B9B8BB78B8C");
		}
	}
}
