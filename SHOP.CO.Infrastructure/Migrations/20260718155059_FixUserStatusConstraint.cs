using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHOP.CO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserStatusConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Status",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Status",
                table: "Users",
                sql: "[Status] IN (N'Unverified', N'Active', N'Locked', N'Deleted')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Status",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Status",
                table: "Users",
                sql: "[Status] IN (N'Active', N'Locked', N'Deleted')");
        }
    }
}
