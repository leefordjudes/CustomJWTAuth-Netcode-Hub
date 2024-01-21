using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using CustomJWTAuth.DTOs;
using CustomJWTAuth.States;
using static CustomJWTAuth.Responses.CustomResponses;

namespace CustomJWTAuth.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient httpClient;
    private const string BaseUrl = "/api/account";

    public AccountService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    private void GetProtectedClient()
    {
        if (Constants.JWTToken == "") throw new AuthenticationException();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.JWTToken);
    }

    private static bool CheckIfUnauthorized(HttpResponseMessage httpResponseMessage)
    {
        return (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized);
    }

    private async Task GetRefreshToken()
    {
        var currentUserSession = new UserSession() { JWTToken = Constants.JWTToken };
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/refresh-token", currentUserSession);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Constants.JWTToken = result!.JWTToken;
    }

    public async Task<LoginResponse> RefreshToken(UserSession userSession)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/refresh-token", userSession);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!;
    }

    public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/register", model);
        var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
        return result!;
    }

    public async Task<LoginResponse> LoginAsync(LoginDTO model)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/login", model);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!;
    }
    
    // public async Task<WeatherForecast[]> GetWeatherForecasts()
    // {
    //     GetProtectedClient();
    //     var response = await httpClient.GetAsync($"{BaseUrl}/weather");
    //     if (CheckIfUnauthorized(response))
    //     {
    //         await GetRefreshToken();
    //         return await GetWeatherForecasts();
    //     }
    //     return await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
    // }

    public async Task<WeatherForecast[]> GetWeatherForecasts()
    {
        GetProtectedClient();
        var response = await httpClient.GetAsync($"{BaseUrl}/weather");
        if (response.StatusCode == HttpStatusCode.Forbidden)
            throw new BadHttpRequestException("Access Violation");
            // return null;
        if (!CheckIfUnauthorized(response))
            return await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
    
        await GetRefreshToken();
        return await GetWeatherForecasts();
    }

    // public async Task<WeatherForecast[]> GetWeatherForecasts()
    // {
    //     if (Constants.JWTToken == "") return null!;
    //     httpClient.DefaultRequestHeaders.Authorization =
    //         new AuthenticationHeaderValue("Bearer", Constants.JWTToken);
    //     return await httpClient.GetFromJsonAsync<WeatherForecast[]>($"{BaseUrl}/weather");
    // }
}