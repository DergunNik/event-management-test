﻿using Application.Dtos.Auth;
using FluentValidation;

namespace WebAPI.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not a valid email address.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}