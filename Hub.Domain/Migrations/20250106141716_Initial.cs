using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                name: "OrganizationalStructure",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbrev = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    IsRoot = table.Column<bool>(type: "bit", nullable: false),
                    IsLeaf = table.Column<bool>(type: "bit", nullable: false),
                    IsDomain = table.Column<bool>(type: "bit", nullable: false),
                    AppearInMobileApp = table.Column<bool>(type: "bit", nullable: false),
                    FatherId = table.Column<long>(type: "bigint", nullable: true),
                    Tree = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalStructure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalStructure_OrganizationalStructure_FatherId",
                        column: x => x.FatherId,
                        principalTable: "OrganizationalStructure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrgStructConfigDefault",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ConfigType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ApplyToRoot = table.Column<bool>(type: "bit", nullable: false),
                    ApplyToDomain = table.Column<bool>(type: "bit", nullable: false),
                    ApplyToLeaf = table.Column<bool>(type: "bit", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SearchName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SearchExtraCondition = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Options = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Legend = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    OrderConfig = table.Column<int>(type: "int", nullable: true),
                    OrgStructConfigDefaultDependencyId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgStructConfigDefault", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgStructConfigDefault_OrgStructConfigDefault_OrgStructConfigDefaultDependencyId",
                        column: x => x.OrgStructConfigDefaultDependencyId,
                        principalTable: "OrgStructConfigDefault",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Document = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OwnerOrgStructId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileGroup",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Administrator = table.Column<bool>(type: "bit", nullable: false),
                    PasswordExpirationDays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowMultipleAccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OwnerOrgStructId = table.Column<long>(type: "bigint", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_OrganizationalStructure_OwnerOrgStructId",
                        column: x => x.OwnerOrgStructId,
                        principalTable: "OrganizationalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalStructureConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationalStructureId = table.Column<long>(type: "bigint", nullable: false),
                    ConfigId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalStructureConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalStructureConfig_OrgStructConfigDefault_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "OrgStructConfigDefault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationalStructureConfig_OrganizationalStructure_OrganizationalStructureId",
                        column: x => x.OrganizationalStructureId,
                        principalTable: "OrganizationalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortalUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Document = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Login = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TempPassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    UserRoleId = table.Column<long>(type: "bigint", nullable: false),
                    DefaultOrgStructureId = table.Column<long>(type: "bigint", nullable: true),
                    OwnerOrgStructId = table.Column<long>(type: "bigint", nullable: false),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangingPass = table.Column<bool>(type: "bit", nullable: false),
                    QrCodeInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AreaCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAccessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPasswordRecoverRequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastUserUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFromApi = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChatUserStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalUser_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PortalUser_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "PortalUserSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortalUserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalUserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalUserSettings_PortalUser_PortalUserId",
                        column: x => x.PortalUserId,
                        principalTable: "PortalUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalStructure_FatherId",
                table: "OrganizationalStructure",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalStructureConfig_ConfigId",
                table: "OrganizationalStructureConfig",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalStructureConfig_OrganizationalStructureId",
                table: "OrganizationalStructureConfig",
                column: "OrganizationalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgStructConfigDefault_OrgStructConfigDefaultDependencyId",
                table: "OrgStructConfigDefault",
                column: "OrgStructConfigDefaultDependencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUser_PersonId",
                table: "PortalUser",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUser_UserRoleId",
                table: "PortalUser",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUserFingerprints_UserId",
                table: "PortalUserFingerprints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUserPassHistory_PortalUserId",
                table: "PortalUserPassHistory",
                column: "PortalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalUserSettings_PortalUserId",
                table: "PortalUserSettings",
                column: "PortalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_OwnerOrgStructId",
                table: "UserRole",
                column: "OwnerOrgStructId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRule");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "LogField");

            migrationBuilder.DropTable(
                name: "OrganizationalStructureConfig");

            migrationBuilder.DropTable(
                name: "PortalUserFingerprints");

            migrationBuilder.DropTable(
                name: "PortalUserPassHistory");

            migrationBuilder.DropTable(
                name: "PortalUserSettings");

            migrationBuilder.DropTable(
                name: "ProfileGroup");

            migrationBuilder.DropTable(
                name: "OrgStructConfigDefault");

            migrationBuilder.DropTable(
                name: "PortalUser");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "OrganizationalStructure");
        }
    }
}
