using System.Security.Claims;
using System.Text;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenProvider(IOptions<TokensOptions> options) : ITokenProvider
{
    public string CreateJwt(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.UserRole.ToString())
        };
        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(options.Value.JwtExpirationInMinutes),
            claims: claims,
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            signingCredentials: 
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(options.Value.Secret)),
                SecurityAlgorithms.HmacSha256));
    
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public string CreateRefresh()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(options.Value.RefreshTokenSize));
    }
}