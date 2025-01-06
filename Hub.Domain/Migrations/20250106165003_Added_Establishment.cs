using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Added_Establishment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Establishment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationalStructureId = table.Column<long>(type: "bigint", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SocialName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CommercialName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CommercialAbbrev = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OpeningTime = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TimezoneIdentifier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TimeZoneDifference = table.Column<int>(type: "int", nullable: true),
                    ClosingTime = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EstablishmentClassifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establishment_OrganizationalStructure_OrganizationalStructureId",
                        column: x => x.OrganizationalStructureId,
                        principalTable: "OrganizationalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncorporationEstablishment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstablishmentId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationalStructureId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IncorporationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncorporationEstablishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncorporationEstablishment_Establishment_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncorporationEstablishment_OrganizationalStructure_OrganizationalStructureId",
                        column: x => x.OrganizationalStructureId,
                        principalTable: "OrganizationalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncorporationEstablishment_PortalUser_UserId",
                        column: x => x.UserId,
                        principalTable: "PortalUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncorporationEstablishmentConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncorporationEstablishmentId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationalStructureId = table.Column<long>(type: "bigint", nullable: false),
                    ConfigId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncorporationEstablishmentConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncorporationEstablishmentConfig_IncorporationEstablishment_IncorporationEstablishmentId",
                        column: x => x.IncorporationEstablishmentId,
                        principalTable: "IncorporationEstablishment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncorporationEstablishmentConfig_OrgStructConfigDefault_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "OrgStructConfigDefault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncorporationEstablishmentConfig_OrganizationalStructure_OrganizationalStructureId",
                        column: x => x.OrganizationalStructureId,
                        principalTable: "OrganizationalStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Establishment_OrganizationalStructureId",
                table: "Establishment",
                column: "OrganizationalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_IncorporationEstablishment_EstablishmentId",
                table: "IncorporationEstablishment",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncorporationEstablishment_OrganizationalStructureId",
                table: "IncorporationEstablishment",
                column: "OrganizationalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_IncorporationEstablishment_UserId",
                table: "IncorporationEstablishment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncorporationEstablishmentConfig_ConfigId",
                table: "IncorporationEstablishmentConfig",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_IncorporationEstablishmentConfig_IncorporationEstablishmentId",
                table: "IncorporationEstablishmentConfig",
                column: "IncorporationEstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncorporationEstablishmentConfig_OrganizationalStructureId",
                table: "IncorporationEstablishmentConfig",
                column: "OrganizationalStructureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncorporationEstablishmentConfig");

            migrationBuilder.DropTable(
                name: "IncorporationEstablishment");

            migrationBuilder.DropTable(
                name: "Establishment");
        }
    }
}
