using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Data.Migrations
{
    
    public partial class relationsBetweenPatientAndBasket : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "BasketItems");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Order_Items",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "unit_price",
                table: "Order_Items",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Order_Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Order_Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedOut",
                table: "CustomerBaskets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "CustomerBaskets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "CustomerBaskets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "BasketItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBaskets_MedicationId",
                table: "CustomerBaskets",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBaskets_PatientId",
                table: "CustomerBaskets",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_MedicationId",
                table: "BasketItems",
                column: "MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Medications_MedicationId",
                table: "BasketItems",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBaskets_AspNetUsers_PatientId",
                table: "CustomerBaskets",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBaskets_Medications_MedicationId",
                table: "CustomerBaskets",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Medications_MedicationId",
                table: "BasketItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBaskets_AspNetUsers_PatientId",
                table: "CustomerBaskets");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBaskets_Medications_MedicationId",
                table: "CustomerBaskets");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBaskets_MedicationId",
                table: "CustomerBaskets");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBaskets_PatientId",
                table: "CustomerBaskets");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_MedicationId",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Order_Items");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Order_Items");

            migrationBuilder.DropColumn(
                name: "IsCheckedOut",
                table: "CustomerBaskets");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "CustomerBaskets");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "CustomerBaskets");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "BasketItems");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Order_Items",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Order_Items",
                newName: "unit_price");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "BasketItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
