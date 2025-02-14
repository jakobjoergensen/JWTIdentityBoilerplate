using FastEndpoints;
using JWT.Api.Endpoints.Dtos;

namespace JWT.Api.Endpoints;

internal class RevokeEndpoint(IdentityContext identityContext) : Endpoint<RevokeRequest>
{
    private readonly IdentityContext _identityContext = identityContext;

    public override void Configure()
    {
        Delete("/Revoke/{Id}");
    }

    public override async Task HandleAsync(RevokeRequest req, CancellationToken ct)
    {
        var refreshToken = _identityContext.RefreshTokens.FirstOrDefault(x => x.Id == req.Id && x.RevokedAt == null);

        if (refreshToken != null)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _identityContext.SaveChangesAsync();
        }
        
        await SendNoContentAsync(ct);
    }
}