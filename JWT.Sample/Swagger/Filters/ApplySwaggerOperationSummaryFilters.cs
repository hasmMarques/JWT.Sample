#region

using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Swagger.Attributes;

#endregion

namespace WebApi.Swagger.Filters
{
	public class ApplySwaggerOperationSummaryFilters : IOperationFilter
	{
		#region Public Methods

		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var attr = context.MethodInfo.CustomAttributes.FirstOrDefault(x =>
				x.AttributeType == typeof(SwaggerOperationSummaryAttribute));
			if (attr != null) operation.Summary = attr.ConstructorArguments.FirstOrDefault().Value.ToString();
		}

		#endregion
	}
}