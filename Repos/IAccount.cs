using CustomJWTAuth.DTOs;
using static CustomJWTAuth.Responses.CustomResponses;

namespace CustomJWTAuth.Repos;

public interface IAccount
{
    Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
    Task<LoginResponse> LoginAsync(LoginDTO model);

    LoginResponse RefreshToken(UserSession userSession);
}