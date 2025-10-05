using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace WebApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryAndCategoryGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "public",
                table: "CategoryGroup",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "Description" });

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "public",
                table: "Category",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGroup_SearchVector",
                schema: "public",
                table: "CategoryGroup",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Category_SearchVector",
                schema: "public",
                table: "Category",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategoryGroup_SearchVector",
                schema: "public",
                table: "CategoryGroup");

            migrationBuilder.DropIndex(
                name: "IX_Category_SearchVector",
                schema: "public",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "public",
                table: "CategoryGroup");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "public",
                table: "Category");
        }
    }
}
