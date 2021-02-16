#region

using System;
using AppSetting.Interfaces;
using Microsoft.Extensions.Configuration;

#endregion

namespace AppSetting.Services
{
	public class AppSetting : IAppSetting
	{
		#region Public Methods

		public string GetStringFromKeyValue(string key)
		{
			return GetConfig().GetSection(key).Value;
		}

		public bool GetBooleanFromKeyValue(string key)
		{
			var value = GetStringFromKeyValue(key);

			if (!bool.TryParse(value, out var result))
				throw new Exception($"Value of Application Setting '{key}' is not a boolean value: {value}");

			return result;
		}

		public int GetIntegerFromKeyValue(string key)
		{
			var value = GetStringFromKeyValue(key);

			if (!int.TryParse(value, out var result))
				throw new Exception($"Value of Application Setting '{key}' is not a integer value: {value}");

			return result;
		}

		public DateTime GetDateTimeFromKeyValue(string key)
		{
			var value = GetStringFromKeyValue(key);

			if (!DateTime.TryParse(value, out var result))
				throw new Exception($"Value of Application Setting '{key}' is not a DateTime value: {value}");

			return result;
		}

		public string GetConnectionString(string connectionName)
		{
			return GetConfig()[connectionName];
		}

		#endregion

		#region Private Methods

		private IConfiguration GetConfig()
		{
			var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", true, true);
			return builder.Build();
		}

		#endregion
	}
}