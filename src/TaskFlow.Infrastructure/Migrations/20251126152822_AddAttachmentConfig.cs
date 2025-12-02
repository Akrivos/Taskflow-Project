using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskItemId1",
                table: "Attachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TaskItemId1",
                table: "Attachments",
                column: "TaskItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tasks_TaskItemId1",
                table: "Attachments",
                column: "TaskItemId1",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tasks_TaskItemId1",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_TaskItemId1",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "TaskItemId1",
                table: "Attachments");
        }
    }
}
