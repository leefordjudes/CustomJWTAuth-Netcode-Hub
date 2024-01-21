using CustomJWTAuth.DTOs;
using static CustomJWTAuth.Responses.CustomResponses;

namespace CustomJWTAuth.Services;

public interface IAccountService
{
    Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
    Task<LoginResponse> LoginAsync(LoginDTO model);
    
    Task<LoginResponse> RefreshToken(UserSession userSession);
    Task<WeatherForecast[]> GetWeatherForecasts();
    
    
}