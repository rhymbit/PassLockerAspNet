using Microsoft.EntityFrameworkCore.Migrations;

namespace PassLockerDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    email = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password_salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    secret_salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    secret_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    confirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    location = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    member_since = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "14/08/2021")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_password",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    domain_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password_salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(40)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_password", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_password_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_password_UserId1",
                table: "user_password",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_password");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
