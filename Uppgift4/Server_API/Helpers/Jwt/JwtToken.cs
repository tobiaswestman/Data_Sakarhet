using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Server_API.Helpers.Jwt;

public class JwtToken
{
    private readonly IConfiguration _configuration;

    public JwtToken(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Generates a JWT token with the specified claims and expiration time.
    public string GenerateToken(ClaimsIdentity claimsIdentity, DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["TokenValidation:Issuer"],
            Audience = _configuration["TokenValidation:Audience"],
            Subject = claimsIdentity,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["TokenValidation:SecretKey"]!)),
                SecurityAlgorithms.HmacSha512Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(securityTokenDescriptor));
    }

    // Generates a device-specific JWT token.
    public string GenerateDeviceToken(string unitId, string deviceId)
    {
        var claims = new[]
            {
                new Claim("UnitId", unitId),
                new Claim("DeviceId", deviceId),
                new Claim("Permission", "Write")
            };

        var claimsIdentity = new ClaimsIdentity(claims);

        return GenerateToken(claimsIdentity, DateTime.Now.AddMinutes(30));
    }
}