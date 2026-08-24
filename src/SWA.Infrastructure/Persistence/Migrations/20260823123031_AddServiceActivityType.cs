using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceActivityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceActivityTypeId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceActivityTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Slug = table.Column<string>(type: "varchar(200)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceActivityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceActivityTypeTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceActivityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Language = table.Column<string>(type: "varchar(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceActivityTypeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceActivityTypeTranslations_ServiceActivityTypes_ServiceActivityTypeId",
                        column: x => x.ServiceActivityTypeId,
                        principalTable: "ServiceActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceActivityTypeId",
                table: "Services",
                column: "ServiceActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceActivityTypeTranslations_ServiceActivityTypeId",
                table: "ServiceActivityTypeTranslations",
                column: "ServiceActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceActivityTypes_ServiceActivityTypeId",
                table: "Services",
                column: "ServiceActivityTypeId",
                principalTable: "ServiceActivityTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceActivityTypes_ServiceActivityTypeId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "ServiceActivityTypeTranslations");

            migrationBuilder.DropTable(
                name: "ServiceActivityTypes");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceActivityTypeId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceActivityTypeId",
                table: "Services");
        }
    }
}
