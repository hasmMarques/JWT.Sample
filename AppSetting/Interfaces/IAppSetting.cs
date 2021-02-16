#region

using System;

#endregion

namespace AppSetting.Interfaces
{
	public interface IAppSetting
	{
		#region Public Methods

		bool GetBooleanFromKeyValue(string key);
		DateTime GetDateTimeFromKeyValue(string key);
		int GetIntegerFromKeyValue(string key);
		string GetStringFromKeyValue(string key);
		string GetConnectionString(string connectionName);

		#endregion
	}
}