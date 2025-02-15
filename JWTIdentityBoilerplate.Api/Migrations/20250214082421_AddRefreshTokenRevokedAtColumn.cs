using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTIdentityBoilerplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenRevokedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Expiration",
                table: "RefreshTokens",
                newName: "ExpiresAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "RefreshTokens",
                newName: "Expiration");
        }
    }
}
