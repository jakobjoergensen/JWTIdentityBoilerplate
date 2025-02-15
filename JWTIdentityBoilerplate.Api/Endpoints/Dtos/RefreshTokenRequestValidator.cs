using FastEndpoints;
using FluentValidation;

namespace JWTIdentityBoilerplate.Api.Endpoints.Dtos;

internal class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
