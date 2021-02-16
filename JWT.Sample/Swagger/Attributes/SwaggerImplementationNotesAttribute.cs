#region

using System;

#endregion

namespace WebApi.Swagger.Attributes
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public class SwaggerImplementationNotesAttribute : Attribute
	{
		#region Properties

		public string ImplementationNotes { get; set; }

		#endregion

		#region Constructor

		public SwaggerImplementationNotesAttribute(string implementationNotes)
		{
			ImplementationNotes = implementationNotes;
		}

		#endregion
	}
}