using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LocationTrackingRepository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    full_address = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<Point>(type: "geometry", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "collection_request",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    driver_id = table.Column<long>(type: "bigint", nullable: true),
                    pickup_location_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    distance_km = table.Column<double>(type: "double precision", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collection_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_collection_request_address_pickup_location_id",
                        column: x => x.pickup_location_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "driver",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    vehicle_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    license_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DropLocations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<long>(type: "bigint", nullable: true),
                    collection_request_id = table.Column<long>(type: "bigint", nullable: false),
                    dump_location_id = table.Column<long>(type: "bigint", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    distance_km = table.Column<double>(type: "double precision", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropLocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_DropLocations_address_dump_location_id",
                        column: x => x.dump_location_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DropLocations_collection_request_collection_request_id",
                        column: x => x.collection_request_id,
                        principalTable: "collection_request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DropLocations_driver_driver_id",
                        column: x => x.driver_id,
                        principalTable: "driver",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "driver_location",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<long>(type: "bigint", nullable: false),
                    location = table.Column<Point>(type: "geometry", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_driver_location_driver_driver_id",
                        column: x => x.driver_id,
                        principalTable: "driver",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    password = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    dob = table.Column<DateOnly>(type: "date", nullable: false),
                    contact_no = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_status", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_status_user_created_by",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_status_user_updated_by",
                        column: x => x.updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_created_by",
                table: "address",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_address_updated_by",
                table: "address",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_address_user_id",
                table: "address",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_collection_request_driver_id",
                table: "collection_request",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_collection_request_pickup_location_id",
                table: "collection_request",
                column: "pickup_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_collection_request_status",
                table: "collection_request",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_collection_request_user_id",
                table: "collection_request",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionStatus_created_by",
                table: "CollectionStatus",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionStatus_updated_by",
                table: "CollectionStatus",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_driver_created_by",
                table: "driver",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_driver_updated_by",
                table: "driver",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_driver_user_id",
                table: "driver",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_driver_location_created_by",
                table: "driver_location",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_driver_location_driver_id",
                table: "driver_location",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_driver_location_status",
                table: "driver_location",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_driver_location_UpdatedBy",
                table: "driver_location",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatus_created_by",
                table: "DriverStatus",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_DriverStatus_updated_by",
                table: "DriverStatus",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_DropLocations_collection_request_id",
                table: "DropLocations",
                column: "collection_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_DropLocations_driver_id",
                table: "DropLocations",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_DropLocations_dump_location_id",
                table: "DropLocations",
                column: "dump_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_created_by",
                table: "roles",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_roles_updated_by",
                table: "roles",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_created_by",
                table: "user",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_status_id",
                table: "user",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_updated_by",
                table: "user",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_status_created_by",
                table: "user_status",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_status_updated_by",
                table: "user_status",
                column: "updated_by");

            migrationBuilder.AddForeignKey(
                name: "FK_address_user_created_by",
                table: "address",
                column: "created_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_address_user_updated_by",
                table: "address",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_address_user_user_id",
                table: "address",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_collection_request_CollectionStatus_status",
                table: "collection_request",
                column: "status",
                principalTable: "CollectionStatus",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_collection_request_driver_driver_id",
                table: "collection_request",
                column: "driver_id",
                principalTable: "driver",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_collection_request_user_user_id",
                table: "collection_request",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionStatus_user_created_by",
                table: "CollectionStatus",
                column: "created_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionStatus_user_updated_by",
                table: "CollectionStatus",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_driver_user_created_by",
                table: "driver",
                column: "created_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_driver_user_updated_by",
                table: "driver",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_driver_user_user_id",
                table: "driver",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_driver_location_DriverStatus_status",
                table: "driver_location",
                column: "status",
                principalTable: "DriverStatus",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_driver_location_user_UpdatedBy",
                table: "driver_location",
                column: "UpdatedBy",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_driver_location_user_created_by",
                table: "driver_location",
                column: "created_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatus_user_created_by",
                table: "DriverStatus",
                column: "created_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverStatus_user_updated_by",
                table: "DriverStatus",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_user_created_by",
                table: "roles",
                column: "created_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_user_updated_by",
                table: "roles",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_status_status_id",
                table: "user",
                column: "status_id",
                principalTable: "user_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_user_created_by",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_user_updated_by",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_status_user_created_by",
                table: "user_status");

            migrationBuilder.DropForeignKey(
                name: "FK_user_status_user_updated_by",
                table: "user_status");

            migrationBuilder.DropTable(
                name: "driver_location");

            migrationBuilder.DropTable(
                name: "DropLocations");

            migrationBuilder.DropTable(
                name: "DriverStatus");

            migrationBuilder.DropTable(
                name: "collection_request");

            migrationBuilder.DropTable(
                name: "CollectionStatus");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "driver");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "user_status");
        }
    }
}
