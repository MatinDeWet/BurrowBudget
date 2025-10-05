using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfCategoryAndCategoryGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryGroup",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SortOrder = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)999),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryGroup_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SortOrder = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)999),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CategoryGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_CategoryGroup_CategoryGroupId",
                        column: x => x.CategoryGroupId,
                        principalSchema: "public",
                        principalTable: "CategoryGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Category_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_CategoryGroupId",
                schema: "public",
                table: "Category",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_UserId",
                schema: "public",
                table: "Category",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGroup_UserId",
                schema: "public",
                table: "CategoryGroup",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CategoryGroup",
                schema: "public");
        }
    }
}
