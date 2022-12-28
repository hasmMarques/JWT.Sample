#region

using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

namespace WebApi.Swagger.Filters
{
    public class AddSwaggerRequestHeadersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes(typeof(SwaggerRequestHeaderAttribute), false);

            foreach (var attr in actionAttributes.OfType<SwaggerRequestHeaderAttribute>())
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Description = attr.Description,
                    In = ParameterLocation.Header,
                    Name = attr.Name,
                    Schema = new OpenApiSchema { Description = attr.Description, Type = attr.Type, Format = attr.Format },
                });
            }
        }
    }
}