using Application.Dtos.Auth;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Mapster;

namespace Application.Mappings;

public class AuthMappings : IRegister
{
    private readonly IPasswordHasher _passwordHasher;
    
    public AuthMappings(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegistrationRequest, User>()
            .Map(dest => dest.PasswordHash, src => _passwordHasher.Hash(src.Password))
            .Map(dest => dest.UserRole, _ => UserRole.DefaultUser)
            .Ignore(dest => dest.Events);
    }
}