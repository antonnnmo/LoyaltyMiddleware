//using LoyaltyMiddleware.Cache;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace RedmondLoyaltyMiddleware.Models.InternalDB
//{
//    public class MiddlewareDBContextFactory : IDesignTimeDbContextFactory<MiddlewareDBContext>
//    {
//        public MiddlewareDBContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<MiddlewareDBContext>();
//            GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.ConnectionString, out string connectionString);
//            optionsBuilder.UseNpgsql(connectionString);
//            return new MiddlewareDBContext(optionsBuilder.Options);
//        }
//    }
//}
