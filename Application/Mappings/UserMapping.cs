using Application.Dtos.User;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class UserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserCreateDto, User>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PasswordHash)
            .Ignore(dest => dest.Events)
            .Ignore(dest => dest.RefreshTokens);

        config.NewConfig<User, UserCreateDto>()
            .Ignore(dest => dest.IsEmailConfirmed); 
        
        config.NewConfig<UserUpdateDto, User>()
            .Ignore(dest => dest.Email)
            .Ignore(dest => dest.IsEmailConfirmed)
            .Ignore(dest => dest.UserRole)
            .Ignore(dest => dest.PasswordHash)
            .Ignore(dest => dest.Events)
            .Ignore(dest => dest.RefreshTokens);
        
        config.NewConfig<User, UserUpdateDto>();
        
        config.NewConfig<UserDto, User>()
            .Ignore(dest => dest.PasswordHash)
            .Ignore(dest => dest.Events)
            .Ignore(dest => dest.RefreshTokens);
        
        config.NewConfig<User, UserDto>();
        
        config.NewConfig<PagedResult<Participant>, UserPageDto>()
            .Ignore(dest => dest.Users);
    }
}