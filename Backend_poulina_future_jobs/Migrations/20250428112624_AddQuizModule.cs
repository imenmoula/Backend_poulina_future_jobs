using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    Duree = table.Column<int>(type: "int", nullable: false),
                    ScoreMinimum = table.Column<int>(type: "int", nullable: false),
                    OffreEmploiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                    table.ForeignKey(
                        name: "FK_Quizzes_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_OffresEmploi_OffreEmploiId",
                        column: x => x.OffreEmploiId,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texte = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    TempsRecommande = table.Column<int>(type: "int", nullable: true),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TentativesQuiz",
                columns: table => new
                {
                    TentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    AppUserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TentativesQuiz", x => x.TentativeId);
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_AspNetUsers_AppUserId1",
                        column: x => x.AppUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reponses",
                columns: table => new
                {
                    ReponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texte = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstCorrecte = table.Column<bool>(type: "bit", nullable: false),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    Explication = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reponses", x => x.ReponseId);
                    table.ForeignKey(
                        name: "FK_Reponses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultatsQuiz",
                columns: table => new
                {
                    ResultatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    QuestionsCorrectes = table.Column<int>(type: "int", nullable: false),
                    NombreQuestions = table.Column<int>(type: "int", nullable: false),
                    TempsTotal = table.Column<int>(type: "int", nullable: false),
                    Reussi = table.Column<bool>(type: "bit", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateResultat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultatsQuiz", x => x.ResultatId);
                    table.ForeignKey(
                        name: "FK_ResultatsQuiz_TentativesQuiz_TentativeId",
                        column: x => x.TentativeId,
                        principalTable: "TentativesQuiz",
                        principalColumn: "TentativeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReponsesUtilisateur",
                columns: table => new
                {
                    ReponseUtilisateurId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TexteReponse = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TempsReponse = table.Column<int>(type: "int", nullable: true),
                    EstCorrecte = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReponsesUtilisateur", x => x.ReponseUtilisateurId);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateur_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateur_Reponses_ReponseId",
                        column: x => x.ReponseId,
                        principalTable: "Reponses",
                        principalColumn: "ReponseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateur_TentativesQuiz_TentativeId",
                        column: x => x.TentativeId,
                        principalTable: "TentativesQuiz",
                        principalColumn: "TentativeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_AppUserId",
                table: "Quizzes",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_OffreEmploiId",
                table: "Quizzes",
                column: "OffreEmploiId");

            migrationBuilder.CreateIndex(
                name: "IX_Reponses_QuestionId",
                table: "Reponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateur_QuestionId",
                table: "ReponsesUtilisateur",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateur_ReponseId",
                table: "ReponsesUtilisateur",
                column: "ReponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateur_TentativeId",
                table: "ReponsesUtilisateur",
                column: "TentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatsQuiz_TentativeId",
                table: "ResultatsQuiz",
                column: "TentativeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_AppUserId",
                table: "TentativesQuiz",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_AppUserId1",
                table: "TentativesQuiz",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_QuizId",
                table: "TentativesQuiz",
                column: "QuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReponsesUtilisateur");

            migrationBuilder.DropTable(
                name: "ResultatsQuiz");

            migrationBuilder.DropTable(
                name: "Reponses");

            migrationBuilder.DropTable(
                name: "TentativesQuiz");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
