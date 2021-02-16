#region

using System;

#endregion

namespace WebApi.Swagger.Attributes
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public class SwaggerOperationSummaryAttribute : Attribute
	{
		#region Properties

		public string OperationSummary { get; set; }

		#endregion

		#region Constructor

		public SwaggerOperationSummaryAttribute(string operationSummary)
		{
			OperationSummary = operationSummary;
		}

		#endregion
	}
}