#region

using System;
using System.Net.Http;
using AppSetting.Interfaces;
using ClientApi.Services;
using JWT.Interfaces;
using JWT.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Client.Sample
{
	public class Program
	{
		#region Fields

		private static ServiceProvider serviceProvider;
		private static IJWTService jwt;

		#endregion

		#region Private Methods

		private static void Main(string[] args)
		{
			GetServices();

			InvokeWeatherForecastService();
		}

		private static void InvokeWeatherForecastService()
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", jwt.GetToken());

			var client = new WeatherServiceClient(
				"https://localhost:44365/",
				httpClient);
			var forecast = client.WeatherForecastAsync().Result;

			foreach (var item in forecast) Console.WriteLine($"{item.Summary}");
		}

		private static void GetServices()
		{
			serviceProvider = ConfigureServices();

			//do the actual work here
			jwt = serviceProvider.GetService<IJWTService>();
		}

		private static ServiceProvider ConfigureServices()
		{
			//setup our DI
			var serviceProvider = new ServiceCollection()
				.AddSingleton<IJWTService, JWTService>()
				.AddSingleton<IAppSetting, AppSetting.Services.AppSetting>()
				.BuildServiceProvider();
			return serviceProvider;
		}

		#endregion
	}
}