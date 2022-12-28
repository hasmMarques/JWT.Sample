#region

using System;

#endregion

namespace WebApi
{
    public class WeatherForecast
	{
		#region Properties

		public DateTime Date { get; set; }

		public int TemperatureC { get; set; }

		public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

		public string Summary { get; set; }

		#endregion
	}
}