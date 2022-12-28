#region

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppSetting.Interfaces;
using JWT.Interfaces;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace JWT.Services
{
    public class JWTService : IJWTService
    {
        #region Fields

        private readonly IAppSetting _appSetting;

        #endregion

        #region Constructor

        public JWTService(IAppSetting appSetting)
        {
            _appSetting = appSetting ?? throw new ArgumentNullException(nameof(appSetting));
        }

        #endregion

        #region Public Methods

        public string GetToken()
        {
            var secretKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_appSetting.GetStringFromKeyValue("JTW:IssuerSigningKey")));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

            var tokeOptions = new JwtSecurityToken(
                issuer: _appSetting.GetStringFromKeyValue("JTW:ValidIssuer"),
                audience: _appSetting.GetStringFromKeyValue("JTW:ValidAudience"),
                claims: new List<Claim>(),
                notBefore: new DateTimeOffset(DateTime.UtcNow).DateTime,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signinCredentials
            );

            var writeToken = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return $"Bearer {writeToken}";
        }

        public void WriteLineToken()
        {
            Console.WriteLine(GetToken());
        }

        #endregion
    }
}
