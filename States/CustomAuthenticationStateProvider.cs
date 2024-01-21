using System.Security.Claims;
using CustomJWTAuth.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace CustomJWTAuth.States;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    private static ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
    {
        if (claims.Email is null)
            return new ClaimsPrincipal();

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.Name, claims.Name!),
                    new(ClaimTypes.Email, claims.Email!),
                },
                "JwtAuth"
                )
            );
    }

    public void UpdateAuthenticationState(string jwtToken)
    {
        var claimsPrincipal = new ClaimsPrincipal();
        if (!string.IsNullOrEmpty(jwtToken))
        {
            Constants.JWTToken = jwtToken;
            // you can store token locally in any form
            var getUserClaims = JWTService.DecryptToken(jwtToken);
            claimsPrincipal = SetClaimPrincipal(getUserClaims);
        }
        else
        {
            Constants.JWTToken = null!;
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(Constants.JWTToken))
                throw new Exception();
                // return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = JWTService.DecryptToken(Constants.JWTToken);
            if (getUserClaims == null)
                throw new Exception();
                // return await Task.FromResult(new AuthenticationState(anonymous));

            var claimsPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(anonymous));
        }
    }
}