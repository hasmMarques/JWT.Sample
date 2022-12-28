#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#endregion

namespace WebApi.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		#region Fields

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		#endregion

		#region Constructor

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns>HttpStatusCode.OK (200) if all is OK. HttpStatusCode.Unauthorized (401) if JWT is invalid.</returns>
        [HttpGet(Name = "WeatherForecast")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IEnumerable<WeatherForecast> Get()
		{
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
				{
					Date = DateTime.Now.AddDays(index),
					TemperatureC = rng.Next(-20, 55),
					Summary = Summaries[rng.Next(Summaries.Length)]
				})
				.ToArray();
		}

		#endregion
	}
}