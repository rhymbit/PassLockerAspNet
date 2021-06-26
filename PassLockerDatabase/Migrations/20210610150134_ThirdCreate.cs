using Microsoft.EntityFrameworkCore.Migrations;

namespace PassLockerDatabase.Migrations
{
    public partial class ThirdCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "secret_answer_hash",
                table: "users",
                newName: "user_secret_answer_hash");

            migrationBuilder.RenameColumn(
                name: "confirmed",
                table: "users",
                newName: "user_confirmed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_secret_answer_hash",
                table: "users",
                newName: "secret_answer_hash");

            migrationBuilder.RenameColumn(
                name: "user_confirmed",
                table: "users",
                newName: "confirmed");
        }
    }
}
