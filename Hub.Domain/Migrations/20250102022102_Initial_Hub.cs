using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Hub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessRule",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    KeyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Tree = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbrev = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Mask = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MinLength = table.Column<long>(type: "bigint", nullable: false),
                    MaxLength = table.Column<long>(type: "bigint", nullable: false),
                    SpecialDocumentValidation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: true),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LogVersion = table.Column<int>(type: "int", nullable: true),
                    FatherId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogField",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogId = table.Column<long>(type: "bigint", nullable: true),
                    PropertyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogField", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileGroup",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Administrator = table.Column<bool>(type: "bit", nullable: false),
                    AllowMultipleAccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Document = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocumentTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    QrCodeInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastUserUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortalUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Login = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    TempPassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangingPass = table.Column<bool>(type: "bit", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastAccessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordRecoverRequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalUser_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PortalUserFingerprints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BrowserInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CookieEnabled = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalUserFingerprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalUserFingerprints_PortalUser_UserId",
                        column: x => x.UserId,
                        principalTable: "PortalUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortalUserPassHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortalUserId = table.Column<long>(type: "bigint", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalUserPassHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalUserPassHistory_PortalUser_PortalUserId",
                        column: x => x.PortalUserId,
                        principalTable: "PortalUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_DocumentTypeId",
                table: "Person",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUser_PersonId",
                table: "PortalUser",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUserFingerprints_UserId",
                table: "PortalUserFingerprints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUserPassHistory_PortalUserId",
                table: "PortalUserPassHistory",
                column: "PortalUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRule");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "LogField");

            migrationBuilder.DropTable(
                name: "PortalUserFingerprints");

            migrationBuilder.DropTable(
                name: "PortalUserPassHistory");

            migrationBuilder.DropTable(
                name: "ProfileGroup");

            migrationBuilder.DropTable(
                name: "PortalUser");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "DocumentType");
        }
    }
}
