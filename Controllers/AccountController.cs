using CustomJWTAuth.DTOs;
using CustomJWTAuth.Repos;
using Microsoft.AspNetCore.Authorization;
using static CustomJWTAuth.Responses.CustomResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomJWTAuth.Controllers
{
    
    
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount accountRepo;

        public AccountController(IAccount accountRepo)
        {
            this.accountRepo = accountRepo;
        }
        
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public ActionResult<LoginResponse> RefreshAsync(UserSession model)
        {
            var result = accountRepo.RefreshToken(model);
            return Ok(result);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegisterDTO model)
        {
            var result = await accountRepo.RegisterAsync(model);
            return Ok(result);
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginDTO model)
        {
            var result = await accountRepo.LoginAsync(model);
            return Ok(result);
        }
        
        [Authorize(Roles="Admin")]
        [HttpGet("weather")]
        public async Task<ActionResult<WeatherForecast[]>> GetWeatherForecast()
        {
            await Task.Delay(500);
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var count = Random.Shared.Next(5, 10);
            var forecasts = Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToArray();
            return Ok(forecasts);
        }
        
    }
}




















