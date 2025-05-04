using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Auth.Helpers;

public class TokenProvider : ITokenProvider
{
    private readonly TokensOptions _options;

    public TokenProvider(IOptions<TokensOptions> options)
    {
        _options = options.Value;
    }

    public string CreateJwt(Domain.Entities.User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.UserRole.ToString())
        };
        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(_options.JwtExpirationInMinutes),
            claims: claims,
            issuer: _options.Issuer,
            audience: _options.Audience,
            signingCredentials:
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.Secret)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public string CreateRefresh()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(_options.RefreshTokenSize));
    }
}