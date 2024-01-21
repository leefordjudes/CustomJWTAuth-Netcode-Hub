using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

using CustomJWTAuth.DTOs;
using CustomJWTAuth.Data;
using CustomJWTAuth.Models;
using CustomJWTAuth.Responses;
using CustomJWTAuth.States;
using Microsoft.IdentityModel.Tokens;
using static CustomJWTAuth.Responses.CustomResponses;

namespace CustomJWTAuth.Repos;

public class Account : IAccount
{
    private readonly AppDbContext appDbContext;
    private readonly IConfiguration config;

    public Account(AppDbContext appDbContext, IConfiguration config)
    {
        this.appDbContext = appDbContext;
        this.config = config;
    }

    private string GenerateToken(ApplicationUser user)
    {
        if (user.Name is null || user.Email is null) throw new UnauthorizedAccessException();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            // new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role!)
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"]!,
            audience: config["Jwt:Audience"]!,
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: credentials
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public LoginResponse RefreshToken(UserSession userSession)
    {
        CustomUserClaims customUserClaims = JWTService.DecryptToken(userSession.JWTToken);
        if (customUserClaims is null)
            return new LoginResponse(false, "Invalid token");    
            
        string newToken = GenerateToken(new ApplicationUser()
            {Name = customUserClaims.Name, Email = customUserClaims.Email});
        return new LoginResponse(true, "New token", newToken);
    }

    private async Task<ApplicationUser> GetUser(string email) =>
        await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    
    public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
    {
        var findUser = await GetUser(model.Email);
        if (findUser != null) return new RegistrationResponse(false, "User already exists");

        var newUser = new ApplicationUser()
        {
            Name = model.Name,
            Email = model.Email,
            Role = model.Role,
            Password = HashPassword(model.Password)
        };
        appDbContext.Users.Add(newUser);
        await appDbContext.SaveChangesAsync();
        return new RegistrationResponse(true, "Success");
    }

    public async Task<LoginResponse> LoginAsync(LoginDTO model)
    {
        var findUser = await GetUser(model.Email);
        if (findUser == null) 
            return new LoginResponse(false, "User not found");
        
        if (!Verify(model.Password, findUser.Password))
            return new LoginResponse(false, "Email/Password not valid");

        string jwtToken = GenerateToken(findUser);
        return new LoginResponse(true, "Login Success", jwtToken);
    }
}