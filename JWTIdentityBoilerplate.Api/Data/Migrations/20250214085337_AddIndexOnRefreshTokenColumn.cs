using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTIdentityBoilerplate.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexOnRefreshTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "RefreshTokens",
                type: "nchar(88)",
                fixedLength: true,
                maxLength: 88,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(88)",
                oldMaxLength: 88);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_RefreshToken",
                table: "RefreshTokens",
                column: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_RefreshToken",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "RefreshTokens",
                type: "nvarchar(88)",
                maxLength: 88,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(88)",
                oldFixedLength: true,
                oldMaxLength: 88);
        }
    }
}
