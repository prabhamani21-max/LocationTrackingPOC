using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocationTrackingRepository.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed User
            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "name", "email", "password", "status_id", "role_id", "gender", "dob", "contact_no", "created_date", "created_by" },
                values: new object[] { 1L, "Santosh", "santosh@gmail.com", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", 1, 1, 1, new DateOnly(1990, 1, 1), "9876543210", now, 1L });

            // Seed UserStatus
            migrationBuilder.InsertData(
                table: "user_status",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 1, "Active", true, now, null });
            migrationBuilder.InsertData(
                table: "user_status",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 2, "Inactive", true, now, null });
            migrationBuilder.InsertData(
                table: "user_status",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 3, "Deleted", false, now, null });

            // Seed Roles
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "role_name", "status_id", "created_date", "created_by" },
                values: new object[] { 1, "Client", 1, now, null });
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "role_name", "status_id", "created_date", "created_by" },
                values: new object[] { 2, "Driver", 1, now, null });

            // Seed DriverStatus
            migrationBuilder.InsertData(
                table: "DriverStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 1, "Online", true, now, null });
            migrationBuilder.InsertData(
                table: "DriverStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 2, "Offline", true, now, null });
            migrationBuilder.InsertData(
                table: "DriverStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 3, "Busy", true, now, null });
            migrationBuilder.InsertData(
                table: "DriverStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 4, "OfflineExit", true, now, null });
            migrationBuilder.InsertData(
                table: "DriverStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 5, "OnlineEntry", true, now, null });

            // Seed CollectionStatus
            migrationBuilder.InsertData(
                table: "CollectionStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 1, "Pending", true, now, null });
            migrationBuilder.InsertData(
                table: "CollectionStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 2, "InProgress", true, now, null });
            migrationBuilder.InsertData(
                table: "CollectionStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 3, "Completed", true, now, null });
            migrationBuilder.InsertData(
                table: "CollectionStatus",
                columns: new[] { "id", "name", "is_active", "created_date", "created_by" },
                values: new object[] { 4, "Cancelled", true, now, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
