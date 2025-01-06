using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Domain.Migrations.AdminDb
{
    /// <inheritdoc />
    public partial class Admin_Added_New_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Schema",
                schema: "admin",
                table: "Tenants");

            migrationBuilder.CreateTable(
                name: "AppConfigurations",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SchemaId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Environment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false),
                    MinVersionIOS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MinVersionAndroid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LatestVersionIOS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LatestVersionAndroid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    InfoAppStoreIOS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    InfoAppStoreAndroid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantBinding",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantBinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantBinding_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "admin",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantBinding_TenantId",
                schema: "admin",
                table: "TenantBinding",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfigurations",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "TenantBinding",
                schema: "admin");

            migrationBuilder.AddColumn<string>(
                name: "Schema",
                schema: "admin",
                table: "Tenants",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
