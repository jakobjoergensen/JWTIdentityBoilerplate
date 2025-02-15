using FastEndpoints;
using FluentValidation;

namespace JWTIdentityBoilerplate.Api.Endpoints.Dtos;

internal class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().When(x => string.IsNullOrEmpty(x.Username));
        RuleFor(x => x.Username).NotEmpty().When(x => string.IsNullOrEmpty(x.Email));
    }
}