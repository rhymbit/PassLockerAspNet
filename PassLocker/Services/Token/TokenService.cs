using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Threading;
using Microsoft.IdentityModel.Tokens;

namespace PassLocker.Services.Token
{
    public class TokenService : ITokenService
    {
        private const string Issuer = "https://localhost:5001";
        private const string Audience = "http://localhost:3000";
        private const int ExpirationTime = 60;

        /// <summary>
        /// Generates a json web token.
        /// </summary>
        /// <param name="username">Current user's username. Fetch if from database and pass it to this method.</param>
        /// <param name="sec">Fetch user's password's hash value and pass it as this argument.</param>
        /// <returns>String value of the generated jwt.</returns>
        public string CreateToken(string username, string sec)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            var securityKey = new
                Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.Default.GetBytes(sec));

            var signingCredentials = new
                Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey,
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken) tokenHandler.CreateJwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(ExpirationTime),
                signingCredentials: signingCredentials
            );

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token, string sec)
        {
            var securityKey = new
                Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.Default.GetBytes(sec));

            var handler = new JwtSecurityTokenHandler();

            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                ValidAudience = Audience,
                ValidIssuer = Issuer,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                LifetimeValidator = LifeTimeValidator,
                IssuerSigningKey = securityKey
            };
            SecurityToken securityToken;
            try
            {
                Thread.CurrentPrincipal = handler.ValidateToken(token,
                    parameters, out securityToken);
            }
            // basically if any exception is thrown, then token is invalid,
            // so we don't care about a specific exception here
            catch (Exception exp)
            {
                return false;
            }

            return true;
        }

        private static bool LifeTimeValidator(DateTime? notBefore, DateTime? expires,
            SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires == null) return false;
            return DateTime.UtcNow < expires;
        }
    }
}