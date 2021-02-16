#region

using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Swagger.Attributes;

#endregion

namespace WebApi.Swagger.Filters
{
	public class ApplySwaggerImplementationNotesFilters : IOperationFilter
	{
		#region Public Methods

		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var attr = context.MethodInfo.CustomAttributes.FirstOrDefault(x =>
				x.AttributeType == typeof(SwaggerImplementationNotesAttribute));
			if (attr != null) operation.Description = attr.ConstructorArguments.FirstOrDefault().Value.ToString();
		}

		#endregion
	}
}