#region

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

#endregion

namespace WebApi.HealthChecks
{
	public class ApiHealthCheck : IHealthCheck
	{
		#region Public Methods

		public Task<HealthCheckResult> CheckHealthAsync(
			HealthCheckContext context,
			CancellationToken cancellationToken = new CancellationToken())
		{
			//TODO: Implement your own healthcheck logic here
			var isHealthy = true;
			return Task.FromResult(isHealthy
				? HealthCheckResult.Healthy("I am one healthy microservice API.")
				: HealthCheckResult.Unhealthy("I am the sad, unhealthy microservice API."));
		}

		#endregion
	}
}