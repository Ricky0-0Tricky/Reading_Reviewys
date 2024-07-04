using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reading_Reviewys.Data.Migrations
{
    /// <inheritdoc />
    public partial class VersaoInicialModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Livro",
                columns: table => new
                {
                    IdLivro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnoPublicacao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro", x => x.IdLivro);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data_Entrada = table.Column<DateOnly>(type: "date", nullable: false),
                    Imagem_Perfil = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data_Subscricao = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "AutorLivro",
                columns: table => new
                {
                    ListaAutoresIdUser = table.Column<int>(type: "int", nullable: false),
                    ListaLivrosIdLivro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorLivro", x => new { x.ListaAutoresIdUser, x.ListaLivrosIdLivro });
                    table.ForeignKey(
                        name: "FK_AutorLivro_Livro_ListaLivrosIdLivro",
                        column: x => x.ListaLivrosIdLivro,
                        principalTable: "Livro",
                        principalColumn: "IdLivro",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutorLivro_Utilizador_ListaAutoresIdUser",
                        column: x => x.ListaAutoresIdUser,
                        principalTable: "Utilizador",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    IdReview = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescricaoReview = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAlteracao = table.Column<DateOnly>(type: "date", nullable: false),
                    UtilizadorFK = table.Column<int>(type: "int", nullable: false),
                    LivroFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.IdReview);
                    table.ForeignKey(
                        name: "FK_Reviews_Livro_LivroFK",
                        column: x => x.LivroFK,
                        principalTable: "Livro",
                        principalColumn: "IdLivro",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Utilizador_UtilizadorFK",
                        column: x => x.UtilizadorFK,
                        principalTable: "Utilizador",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewFK = table.Column<int>(type: "int", nullable: false),
                    CriadorComentarioFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Reviews_ReviewFK",
                        column: x => x.ReviewFK,
                        principalTable: "Reviews",
                        principalColumn: "IdReview",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Utilizador_CriadorComentarioFK",
                        column: x => x.CriadorComentarioFK,
                        principalTable: "Utilizador",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutorLivro_ListaLivrosIdLivro",
                table: "AutorLivro",
                column: "ListaLivrosIdLivro");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_CriadorComentarioFK",
                table: "Comentarios",
                column: "CriadorComentarioFK");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ReviewFK",
                table: "Comentarios",
                column: "ReviewFK");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_LivroFK",
                table: "Reviews",
                column: "LivroFK");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UtilizadorFK",
                table: "Reviews",
                column: "UtilizadorFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutorLivro");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Livro");

            migrationBuilder.DropTable(
                name: "Utilizador");
        }
    }
}
