//#region

//using System;
//using System.Threading.Tasks;
//using JWT.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using WebApi.Swagger.Attributes;

//#endregion

//namespace WebApi.Controllers
//{
//	public class JWTController : ControllerBase
//	{
//		#region Fields

//		private readonly IJWTService _jtwTokenService;

//		#endregion

//		#region Constructor

//		public JWTController(IJWTService jtwTokenService)
//		{
//			_jtwTokenService = jtwTokenService ?? throw new ArgumentNullException(nameof(jtwTokenService));
//		}

//		#endregion

//		#region Public Methods

//		[Obsolete("This method is deprecated do not use it!!!", true)]
//		[SwaggerImplementationNotes("NOT TO BE USED!!!")]
//		[SwaggerOperationSummary("NOT TO BE USED!!!")]
//		[HttpGet]
//		[Route("jwt", Name = "jwt")]
//		[Produces("application/json")]
//		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
//		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
//		public async Task<IActionResult> JWT()
//		{
//			try
//			{
//				return Ok(_jtwTokenService.GetToken());
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(500);
//			}
//		}

//		#endregion

//		#region Private Methods

//		private void LogException(string callerMember, Exception ex)
//		{
//			//Log somewhere...
//		}

//		#endregion
//	}
//}
