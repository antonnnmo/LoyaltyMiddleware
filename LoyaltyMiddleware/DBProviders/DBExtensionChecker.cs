using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondLoyaltyMiddleware.DBProviders
{
	public static class DBExtensionChecker
	{
        public static IWebHost CheckDatabase(this IWebHost webHost)
        {
            throw new NotImplementedException();
            return webHost;
        }
    }
}
