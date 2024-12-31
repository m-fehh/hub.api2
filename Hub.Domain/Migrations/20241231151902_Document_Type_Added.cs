using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hub.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Document_Type_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentType");
        }
    }
}
