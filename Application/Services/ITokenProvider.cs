using Domain.Entities;

namespace Application.Services;

public interface ITokenProvider
{
    string CreateJwt(User user);
    string CreateRefresh();
}