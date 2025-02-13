using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWT.Api.Data;

public class IdentityContext : IdentityDbContext<ApiUser>
{
    public DbSet<RefreshTokens> RefreshTokens { get; set; }

    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<RefreshTokens>().ToTable("RefreshTokens");
        builder.Entity<RefreshTokens>().HasKey(x => x.Id);
        builder.Entity<RefreshTokens>().Property(x => x.RefreshToken).HasMaxLength(88).IsRequired();
        builder.Entity<RefreshTokens>().Property(x => x.Expiration).IsRequired();

        base.OnModelCreating(builder);
    }
}