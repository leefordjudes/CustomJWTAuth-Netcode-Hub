namespace CustomJWTAuth.DTOs;

public record CustomUserClaims(string Name = null!, string Email = null!, string Role = null!);
