#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using WebApi.HealthChecks;
using WebApi.Swagger;
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

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                //that's to the method you created
                ResponseWriter = WriteHealthCheckResponse
            });

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

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

        private static void ConfigureSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Pattern = new DateTime(2022, 12, 31).ToString("yyyy-MM-ddTHH:mm:ss.fff") });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT.Sample", Version = "v1" });

                // Add security to swagger doc
                c.AddSecurityDefinition(SwaggerConfiguration.SecurityScheme.Name, SwaggerConfiguration.SecurityScheme);
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.OperationFilter<AddSwaggerRequestHeadersFilter>();

                // Set the comments path for the Swagger JSON and UI.
                List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile));
            });
        }

        private static Task WriteHealthCheckResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions { Indented = true };

            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("status", healthReport.Status.ToString());
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);
                    jsonWriter.WriteString("status",
                        healthReportEntry.Value.Status.ToString());
                    jsonWriter.WriteString("description",
                        healthReportEntry.Value.Description);
                    jsonWriter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value,
                            item.Value?.GetType() ?? typeof(object));
                    }

                    jsonWriter.WriteEndObject();
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return context.Response.WriteAsync(
                Encoding.UTF8.GetString(memoryStream.ToArray()));
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

        #endregion
    }
}
