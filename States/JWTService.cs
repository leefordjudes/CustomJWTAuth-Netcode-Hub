using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CustomJWTAuth.DTOs;

namespace CustomJWTAuth.States;

public static class JWTService
{
    public static CustomUserClaims DecryptToken(string jwtToken)
    {
        try
        {
            if (string.IsNullOrEmpty(jwtToken))
                return new CustomUserClaims();
            
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);
            return new CustomUserClaims(name!.Value, email!.Value);
        }
        catch 
        {
            return null!;
        }
        
    }
}