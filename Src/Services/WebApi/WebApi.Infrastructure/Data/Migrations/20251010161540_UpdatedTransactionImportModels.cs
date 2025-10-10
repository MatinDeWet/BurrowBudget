using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTransactionImportModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_ImportBatchId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawAmount",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                schema: "import",
                table: "TransactionImportRow",
                newName: "StatusAtUtc");

            migrationBuilder.RenameColumn(
                name: "AmountMinor",
                schema: "import",
                table: "TransactionImportRow",
                newName: "SignedMinor");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "import",
                table: "TransactionImportRow",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RawType",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RawFitId",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RawDescription",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "RawDate",
                schema: "import",
                table: "TransactionImportRow",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Counterparty",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DuplicateOfRowId",
                schema: "import",
                table: "TransactionImportRow",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorCode",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorLogJson",
                schema: "import",
                table: "TransactionImportRow",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MatchConfidence",
                schema: "import",
                table: "TransactionImportRow",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payee",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawAmountText",
                schema: "import",
                table: "TransactionImportRow",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawBalanceText",
                schema: "import",
                table: "TransactionImportRow",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawCounterparty",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawCurrency",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawHash",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RawLineNumber",
                schema: "import",
                table: "TransactionImportRow",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RawRecordJson",
                schema: "import",
                table: "TransactionImportRow",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RawReference",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SuggestedCategoryId",
                schema: "import",
                table: "TransactionImportRow",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ExternalId",
                schema: "import",
                table: "TransactionImportRow",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ImportBatchId_RawFitId",
                schema: "import",
                table: "TransactionImportRow",
                columns: new[] { "ImportBatchId", "RawFitId" },
                unique: true,
                filter: "\"RawFitId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ImportBatchId_RawHash",
                schema: "import",
                table: "TransactionImportRow",
                columns: new[] { "ImportBatchId", "RawHash" },
                unique: true,
                filter: "\"RawHash\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ImportBatchId_RawLineNumber",
                schema: "import",
                table: "TransactionImportRow",
                columns: new[] { "ImportBatchId", "RawLineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ImportBatchId_Status",
                schema: "import",
                table: "TransactionImportRow",
                columns: new[] { "ImportBatchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_RawFitId",
                schema: "import",
                table: "TransactionImportRow",
                column: "RawFitId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_RawHash",
                schema: "import",
                table: "TransactionImportRow",
                column: "RawHash");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_Status_StatusAtUtc",
                schema: "import",
                table: "TransactionImportRow",
                columns: new[] { "Status", "StatusAtUtc" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_import_row_currency_len",
                schema: "import",
                table: "TransactionImportRow",
                sql: "\"Currency\" IS NULL OR char_length(\"Currency\") = 3");

            migrationBuilder.AddCheckConstraint(
                name: "ck_import_row_raw_currency_len",
                schema: "import",
                table: "TransactionImportRow",
                sql: "\"RawCurrency\" IS NULL OR char_length(\"RawCurrency\") = 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_ExternalId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_ImportBatchId_RawFitId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_ImportBatchId_RawHash",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_ImportBatchId_RawLineNumber",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_ImportBatchId_Status",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_RawFitId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_RawHash",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropIndex(
                name: "IX_TransactionImportRow_Status_StatusAtUtc",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropCheckConstraint(
                name: "ck_import_row_currency_len",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropCheckConstraint(
                name: "ck_import_row_raw_currency_len",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "Counterparty",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "DuplicateOfRowId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "ErrorCode",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "ErrorLogJson",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "MatchConfidence",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "Payee",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawAmountText",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawBalanceText",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawCounterparty",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawCurrency",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawHash",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawLineNumber",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawRecordJson",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "RawReference",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "Reference",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.DropColumn(
                name: "SuggestedCategoryId",
                schema: "import",
                table: "TransactionImportRow");

            migrationBuilder.RenameColumn(
                name: "StatusAtUtc",
                schema: "import",
                table: "TransactionImportRow",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "SignedMinor",
                schema: "import",
                table: "TransactionImportRow",
                newName: "AmountMinor");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "import",
                table: "TransactionImportRow",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RawType",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RawFitId",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RawDescription",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "RawDate",
                schema: "import",
                table: "TransactionImportRow",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Memo",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawAmount",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                schema: "import",
                table: "TransactionImportRow",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ImportBatchId",
                schema: "import",
                table: "TransactionImportRow",
                column: "ImportBatchId");
        }
    }
}
