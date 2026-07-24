using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProductImageForMinIO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "ProductImages",
                newName: "ObjectKey");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "ProductImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "ProductImages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "ProductImages");

            migrationBuilder.RenameColumn(
                name: "ObjectKey",
                table: "ProductImages",
                newName: "FilePath");
        }
    }
}
