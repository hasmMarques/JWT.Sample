#region

using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

#endregion

namespace WebApi.Swagger.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            var requiredScopesMethod = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            var requiredScopesClass = context.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            if (requiredScopesMethod.Any() || requiredScopesClass.Any())
            {
                var polices = requiredScopesClass.Select(c => c.Policy).Union(requiredScopesMethod.Select(s => s.Policy)).Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList();

                if (polices.Any())
                {
                    string policyTest = $"Required policy: {string.Join(", ", polices)}";
                    operation.Summary = string.IsNullOrEmpty(operation.Summary) ? policyTest : $" - {policyTest}";
                }

                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        SwaggerConfiguration.SecurityScheme, polices
                    },
                });
            }
        }
    }
}