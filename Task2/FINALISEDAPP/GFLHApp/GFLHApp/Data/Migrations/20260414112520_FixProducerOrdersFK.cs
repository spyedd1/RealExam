using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GFLHApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixProducerOrdersFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProducerOrders_Producers_ProducersId",
                table: "ProducerOrders");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Producers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProducersId",
                table: "ProducerOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProducerId",
                table: "ProducerOrders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Producers_UserId",
                table: "Producers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProducerOrders_ProducerId",
                table: "ProducerOrders",
                column: "ProducerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProducerOrders_Producers_ProducerId",
                table: "ProducerOrders",
                column: "ProducerId",
                principalTable: "Producers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProducerOrders_Producers_ProducersId",
                table: "ProducerOrders",
                column: "ProducersId",
                principalTable: "Producers",
                principalColumn: "ProducersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProducerOrders_Producers_ProducerId",
                table: "ProducerOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProducerOrders_Producers_ProducersId",
                table: "ProducerOrders");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Producers_UserId",
                table: "Producers");

            migrationBuilder.DropIndex(
                name: "IX_ProducerOrders_ProducerId",
                table: "ProducerOrders");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Producers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "ProducersId",
                table: "ProducerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProducerId",
                table: "ProducerOrders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ProducerOrders_Producers_ProducersId",
                table: "ProducerOrders",
                column: "ProducersId",
                principalTable: "Producers",
                principalColumn: "ProducersId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
