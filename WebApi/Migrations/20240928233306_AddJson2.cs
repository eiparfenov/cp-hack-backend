using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApi.Models;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddJson2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Highlight>>(
                name: "highlights",
                table: "videos",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "highlights",
                table: "videos",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(List<Highlight>),
                oldType: "jsonb");
        }
    }
}
