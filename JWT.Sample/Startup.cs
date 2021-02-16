#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSetting.Interfaces;
using JWT.Interfaces;
using JWT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.HealthChecks;
using WebApi.Swagger.Filters;

#endregion

namespace WebApi
{
	public class Startup
	{
		#region Fields

		private const string SwaggerService = "SERVICE FOR APP V1";

		#endregion

		#region Properties

		public IConfiguration Configuration { get; }

		#endregion

		#region Constructor

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		#endregion

		#region Public Methods

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			AddServices(services);
			ConfigureJTW(services);
			ConfigureSwagger(services);

			services.AddControllers();

			services.AddHealthChecks()
				.AddCheck<ApiHealthCheck>("api");

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", builder =>
				{
					builder
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials()
						.Build();
				});
			});

			//services.AddSwaggerGen(c =>
			//{
			//	c.SwaggerDoc("v1", new OpenApiInfo {Title = "JWT.Sample", Version = "v1"});
			//});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
				// specifying the Swagger JSON endpoint.
				app.UseSwaggerUI(c =>
				{
					var swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
					c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", SwaggerService);
				});
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseHealthChecks("/health", new HealthCheckOptions
			{
				//that's to the method you created 
				ResponseWriter = WriteHealthCheckResponse
			});

			app.UseAuthorization();
			app.UseAuthentication();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}

		#endregion

		#region Private Methods

		private static void AddServices(IServiceCollection services)
		{
			//External Services
			services.AddTransient<IAppSetting, AppSetting.Services.AppSetting>();
			services.AddTransient<IJWTService, JWTService>();

			//Internal Services
		}


		private void ConfigureJTW(IServiceCollection services)
		{
			services.AddAuthentication(auth =>
				{
					auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey =
							true, //The signing key is valid and is trusted by the server (ValidateIssuerSigningKey=true)
						IssuerSigningKey =
							new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JTW:IssuerSigningKey"])),
						ValidateIssuer =
							true, //The issuer is the actual server that created the token (ValidateIssuer=true)
						ValidIssuer = Configuration["JTW:ValidIssuer"],
						ValidateAudience =
							true, //The receiver of the token is a valid recipient (ValidateAudience=true)
						ValidateLifetime = true, //The token has not expired (ValidateLifetime=true)
						ValidAudience = Configuration["JTW:ValidAudience"],
						RequireExpirationTime = true,
						ClockSkew = TimeSpan.Zero
					};
				});
		}

		private static void ConfigureSwagger(IServiceCollection services)
		{
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo {Title = SwaggerService, Version = "v1"});
				c.OperationFilter<ApplySwaggerImplementationNotesFilters>();
				c.OperationFilter<ApplySwaggerOperationSummaryFilters>();
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description =
						"JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header
						},
						new List<string>()
					}
				});
			});
		}

		private Task WriteHealthCheckResponse(HttpContext httpContext,
			HealthReport result)
		{
			httpContext.Response.ContentType = "application/json";
			var json = new JObject(
				new JProperty("status", result.Status.ToString()),
				new JProperty("results", new JObject(result.Entries.Select(pair =>
					new JProperty(pair.Key, new JObject(
						new JProperty("status", pair.Value.Status.ToString()),
						new JProperty("description", pair.Value.Description),
						new JProperty("data", new JObject(pair.Value.Data.Select(
							p => new JProperty(p.Key, p.Value))))))))));
			return httpContext.Response.WriteAsync(
				json.ToString(Formatting.Indented));
		}

		#endregion
	}
}