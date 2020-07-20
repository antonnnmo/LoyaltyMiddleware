using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using LoyaltyMiddleware.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedmondLoyaltyMiddleware.Models.InternalDB;
using RedmondLoyaltyMiddleware.Schedulle;

namespace LoyaltyMiddleware
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson();
			services.AddMemoryCache();
			services.AddDbContext<MiddlewareDBContext>();
			services.AddTransient<PromocodeAttemptsJob>();

			GlobalCacheReader.LoadFromConfiguration(Configuration);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, MiddlewareDBContext dBContext)
		{
			loggerFactory.AddLog4Net();
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

			dBContext.Database.Migrate();

			JobManager.Initialize(new Scheduller(app.ApplicationServices));
		}
	}
}
