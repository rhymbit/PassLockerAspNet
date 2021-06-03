using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PassLockerDatabase.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    user_email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    user_password_hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    secret_answer_hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    confirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    member_since = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "user_passwords",
                columns: table => new
                {
                    user_password_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    domain_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    domain_password_hash = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_passwords", x => x.user_password_id);
                    table.ForeignKey(
                        name: "FK_user_passwords_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_passwords_user_id",
                table: "user_passwords",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_passwords");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
