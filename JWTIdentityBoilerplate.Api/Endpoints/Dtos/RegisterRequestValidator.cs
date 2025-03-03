﻿using FastEndpoints;
using FluentValidation;

namespace JWTIdentityBoilerplate.Api.Endpoints.Dtos;

internal class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}