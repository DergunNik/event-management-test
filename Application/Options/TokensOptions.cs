namespace Application.Options;

public class TokensOptions
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int JwtExpirationInMinutes { get; set; }
    public int RefreshExpirationInDays { get; set; }
    public int RefreshTokenSize { get; set; }
}