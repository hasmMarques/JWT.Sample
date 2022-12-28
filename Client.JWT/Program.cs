using AppSetting.Interfaces;
using JWT.Interfaces;
using JWT.Services;

namespace Client.JWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
          .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
          .AddJsonFile("appsettings.json", optional: false)
            .Build();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureServices(services => services.AddSingleton<IConfiguration>(configurationRoot))
            .Build()
            .Services
            .GetService<IJWTService>()
            .WriteLineToken();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            //setup our DI
            services
                .AddSingleton<IJWTService, JWTService>()
                .AddSingleton<IAppSetting, AppSetting.Services.AppSetting>();
        }
    }
}
