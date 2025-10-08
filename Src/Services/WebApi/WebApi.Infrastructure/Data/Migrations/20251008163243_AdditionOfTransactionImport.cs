using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfTransactionImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "import");

            migrationBuilder.CreateTable(
                name: "TransactionImportBatch",
                schema: "import",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UploadedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    QueuedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CanceledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SupersededAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionImportBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionImportBatch_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "public",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionImportFile",
                schema: "import",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FileExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BlobContainer = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BlobName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionImportFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionImportFile_TransactionImportBatch_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalSchema: "import",
                        principalTable: "TransactionImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionImportRow",
                schema: "import",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    RawDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RawAmount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RawDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RawFitId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RawType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NormalizedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AmountMinor = table.Column<long>(type: "bigint", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Memo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    StatusReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionImportRow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionImportRow_TransactionImportBatch_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalSchema: "import",
                        principalTable: "TransactionImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportBatch_AccountId",
                schema: "import",
                table: "TransactionImportBatch",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportFile_ImportBatchId",
                schema: "import",
                table: "TransactionImportFile",
                column: "ImportBatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionImportRow_ImportBatchId",
                schema: "import",
                table: "TransactionImportRow",
                column: "ImportBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionImportFile",
                schema: "import");

            migrationBuilder.DropTable(
                name: "TransactionImportRow",
                schema: "import");

            migrationBuilder.DropTable(
                name: "TransactionImportBatch",
                schema: "import");
        }
    }
}
