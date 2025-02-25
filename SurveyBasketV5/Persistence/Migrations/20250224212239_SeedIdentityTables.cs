using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasketV5.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "01953979-ba1d-7364-8bd0-74fb2bce506d", "01953979-ba1d-7364-8bd0-74fcb0f8df17", false, false, "Admin", "ADMIN" },
                    { "01953979-ba1d-7364-8bd0-74fd87abc0e8", "01953999-c1c8-7058-b040-8bf2765537a8", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "01953978-f526-7c3a-b7cd-bbd8a6e31572", 0, "01953979-ba1d-7364-8bd0-74fa0bd96bdf", "admin@survey-basket.com", true, "Ali", "Mustafa", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAELu5I2TfYxOvn01Tux9IF9SXT6jOy9Xk19eVszIUCNHou8uXp7hZ+9RUIc64gDYQdQ==", null, false, "01953979BA1D73648BD074F928A7F7B8", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 2, "permissions", "polls:add", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 3, "permissions", "polls:update", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 4, "permissions", "polls:delete", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 5, "permissions", "questions:read", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 6, "permissions", "questions:add", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 7, "permissions", "questions:update", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 8, "permissions", "questions:delete", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 9, "permissions", "users:read", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 10, "permissions", "users:add", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 11, "permissions", "users:update", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 12, "permissions", "users:delete", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 13, "permissions", "roles:read", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 14, "permissions", "roles:add", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 15, "permissions", "roles:update", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 16, "permissions", "roles:delete", "01953979-ba1d-7364-8bd0-74fb2bce506d" },
                    { 17, "permissions", "results:read", "01953979-ba1d-7364-8bd0-74fb2bce506d" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "01953979-ba1d-7364-8bd0-74fb2bce506d", "01953978-f526-7c3a-b7cd-bbd8a6e31572" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01953979-ba1d-7364-8bd0-74fd87abc0e8");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "01953979-ba1d-7364-8bd0-74fb2bce506d", "01953978-f526-7c3a-b7cd-bbd8a6e31572" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01953979-ba1d-7364-8bd0-74fb2bce506d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "01953978-f526-7c3a-b7cd-bbd8a6e31572");
        }
    }
}
