using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VilaManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renameStatusToOrderStatusInBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Bookings",
                newName: "OrderStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "Bookings",
                newName: "Status");
        }
    }
}
