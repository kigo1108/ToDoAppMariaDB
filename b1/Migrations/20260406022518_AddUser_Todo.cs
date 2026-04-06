using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace b1.Migrations
{
    /// <inheritdoc />
    public partial class AddUser_Todo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "TodoItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_UserID",
                table: "TodoItems",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Users_UserID",
                table: "TodoItems",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserID",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_UserID",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TodoItems");
        }
    }
}
