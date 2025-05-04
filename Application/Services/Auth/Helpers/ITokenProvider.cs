namespace Application.Services.Auth.Helpers;

public interface ITokenProvider
{
    string CreateJwt(Domain.Entities.User user);
    string CreateRefresh();
}