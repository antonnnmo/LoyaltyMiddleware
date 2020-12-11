using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RedmondIntegrationService
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.SqlConnectionString, Configuration.GetConnectionString("Database"));
			GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.ProcessingUri, Configuration.GetValue<string>("ProcessingUri"));
			//GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.ProcessingLogin, Configuration.GetValue<string>("ProcessingLogin"));
			//GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.ProcessingPasword, Configuration.GetValue<string>("ProcessingPasword"));
			GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.PackSize, Configuration.GetSection("IntegrationSettings").GetValue<int>("PackSize"));
			GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.ThreadCount, Configuration.GetSection("IntegrationSettings").GetValue<int>("ThreadCount"));
			GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.CrmRequestTimeout, Configuration.GetSection("IntegrationSettings").GetValue<int>("CrmRequestTimeout"));
			//GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.PersonalAreaLogin, Configuration.GetValue<string>(GlobalCacheReader.CacheKeys.PersonalAreaLogin));
			//GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.PersonalAreaPasword, Configuration.GetValue<string>(GlobalCacheReader.CacheKeys.PersonalAreaPasword));
			GlobalCacheReader.Cache.Set(GlobalCacheReader.CacheKeys.PersonalAreaUri, Configuration.GetValue<string>(GlobalCacheReader.CacheKeys.PersonalAreaUri));

			BaseManager.CreateColumns();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
