using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovalOfCountryAndCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryCurrency",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Country",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Currency",
                schema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                schema: "public",
                columns: table => new
                {
                    Iso2 = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    E164CallingCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    FlagEmoji = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Iso3 = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    IsoNumeric = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Region = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Subregion = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Iso2);
                    table.CheckConstraint("ck_country_iso2", "length(\"Iso2\")=2 AND \"Iso2\" ~ '^[A-Z]{2}$'");
                    table.CheckConstraint("ck_country_iso3", "length(\"Iso3\")=3 AND \"Iso3\" ~ '^[A-Z]{3}$'");
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                schema: "public",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MinorUnits = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Code);
                    table.CheckConstraint("ck_currency_code", "length(\"Code\")=3 AND \"Code\" ~ '^[A-Z]{3}$'");
                });

            migrationBuilder.CreateTable(
                name: "CountryCurrency",
                schema: "public",
                columns: table => new
                {
                    CurrencyCode = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    CountryIso2 = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryCurrency", x => new { x.CurrencyCode, x.CountryIso2 });
                    table.ForeignKey(
                        name: "FK_CountryCurrency_Country_CountryIso2",
                        column: x => x.CountryIso2,
                        principalSchema: "public",
                        principalTable: "Country",
                        principalColumn: "Iso2",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryCurrency_Currency_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "public",
                        principalTable: "Currency",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Country_Iso3",
                schema: "public",
                table: "Country",
                column: "Iso3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_IsoNumeric",
                schema: "public",
                table: "Country",
                column: "IsoNumeric",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_Name",
                schema: "public",
                table: "Country",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CountryCurrency_CountryIso2_CurrencyCode_ValidFrom",
                schema: "public",
                table: "CountryCurrency",
                columns: new[] { "CountryIso2", "CurrencyCode", "ValidFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CountryCurrency_CountryIso2_IsPrimary_ValidFrom",
                schema: "public",
                table: "CountryCurrency",
                columns: new[] { "CountryIso2", "IsPrimary", "ValidFrom" },
                filter: "\"IsPrimary\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_Name",
                schema: "public",
                table: "Currency",
                column: "Name",
                unique: true);
        }
    }
}
