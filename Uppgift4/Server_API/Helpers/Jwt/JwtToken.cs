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

    public string GenerateToken(ClaimsIdentity claimsIdentity, DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["TokenValidation:ValidIssuer"],
            Audience = _configuration["TokenValidation:ValidAudience"],
            Subject = claimsIdentity,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["TokenValidation:SecretKey"]!)),
                SecurityAlgorithms.HmacSha512Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(securityTokenDescriptor));
    }
}