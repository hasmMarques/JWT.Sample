#region

using Microsoft.OpenApi.Models;

#endregion

namespace WebApi.Swagger
{
    internal static class SwaggerConfiguration
    {
        internal static OpenApiSecurityScheme SecurityScheme { get; } = new OpenApiSecurityScheme()
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Authorization: Bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference() { Id = "Authorization", Type = ReferenceType.SecurityScheme },
        };
    }
}