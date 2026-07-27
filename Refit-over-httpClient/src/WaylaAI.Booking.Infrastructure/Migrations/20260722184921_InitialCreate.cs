using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaylaAI.Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Destination = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "Date", "Destination", "Price", "UserId" },
                values: new object[,]
                {
                    { new Guid("6789f2a9-c89b-466d-8c43-f6d8961726a7"), new DateTime(2027, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "New York", 900.50m, "auth0|mocked-user-id" },
                    { new Guid("b11c97f1-7c96-48cf-94f7-e435967bb1d9"), new DateTime(2027, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Tokyo", 2800.00m, "auth0|mocked-user-id" },
                    { new Guid("d343467c-d6b3-4f9e-a868-233bb93efd68"), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paris", 1250.00m, "auth0|mocked-user-id" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
