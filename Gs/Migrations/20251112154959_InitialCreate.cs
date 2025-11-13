using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_cliente_gs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ProfissaoAtual = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Competencias = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cliente_gs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Titulo = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Requisitos = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Empresa = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_job", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_cliente_gs_Email",
                table: "tb_cliente_gs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_cliente_gs");

            migrationBuilder.DropTable(
                name: "tb_job");
        }
    }
}
