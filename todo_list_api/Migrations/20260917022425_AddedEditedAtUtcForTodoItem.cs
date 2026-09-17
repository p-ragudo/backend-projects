using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_list_api.Migrations
{
    /// <inheritdoc />
    public partial class AddedEditedAtUtcForTodoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                schema: "todo_list_api",
                table: "TodoItems",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EditedAtUtc",
                schema: "todo_list_api",
                table: "TodoItems",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "todo_list_api",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "EditedAtUtc",
                schema: "todo_list_api",
                table: "TodoItems");
        }
    }
}
