#region

using System;

#endregion

namespace WebApi.Swagger.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerRequestHeaderAttribute : Attribute
    {
        public SwaggerRequestHeaderAttribute(string name, string type, string description, string format = "")
        {
            Name = name;
            Type = type;
            Description = description;
            Format = format;
        }

        public string Name { get; }

        public string Type { get; }

        public string Description { get; }

        public string Format { get; }
    }
}