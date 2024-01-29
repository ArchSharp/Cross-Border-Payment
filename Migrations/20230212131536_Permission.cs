using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Migrations
{
    public partial class Permission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfileComplete",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name", "RoleId" },
                values: new object[,]
                {
                    { new Guid("b8ff867b-1979-44fb-86da-7ccfce4d052d"), "Transaction View", null },
                    { new Guid("c15dcb6f-a335-4518-afb4-33c83e94bc2f"), "ExchangeRate Delete", null },
                    { new Guid("4e5e641b-d54d-4c8f-a0d1-23bb23bbb6d9"), "ExchangeRate Create", null },
                    { new Guid("aad5c740-0b0b-485e-a488-5128a95d0a55"), "ExchangeRate Edit", null },
                    { new Guid("7de1e463-3e67-4ad0-94b8-09873030dc95"), "ExchangeRate View", null },
                    { new Guid("04eb9f6c-570c-4f75-a1c3-505beb7583ff"), "Location Delete", null },
                    { new Guid("7b476fc8-e1cd-49bb-986d-db9bab72e003"), "Location Create", null },
                    { new Guid("345011ad-de0a-492d-8706-06f877a6e202"), "Location Edit", null },
                    { new Guid("43eac5fd-7509-4cd5-83aa-17083ccbc577"), "Customer Delete", null },
                    { new Guid("b129d9aa-f6a9-4ed4-a50e-f3651a8633dc"), "Location View", null },
                    { new Guid("a2c9fcaa-560b-4bd4-88d7-727ec722e47e"), "Customer View", null },
                    { new Guid("f7ddd8d8-a96e-42f9-92f7-b92a8061aca0"), "Staff Create", null },
                    { new Guid("d8fa6d21-4a0b-40c3-a4e7-0ab7a4559c49"), "Staff Delete", null },
                    { new Guid("6989d9bc-b5df-4088-8e8e-e452bba3c098"), "Staff Edit", null },
                    { new Guid("65ef9fb5-0a0f-4805-a712-66983b9ca33c"), "Staff View", null },
                    { new Guid("b1f5a953-a3c0-40b7-9842-3ed29084aa27"), "Transaction Delete", null },
                    { new Guid("22d17c90-7658-4120-a087-c1f30797f9f5"), "Transaction Edit", null },
                    { new Guid("0a31d551-2a1c-4941-aeec-a698b6678747"), "Customer Edit", null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("ae25cb3d-b983-44eb-a233-430684f37330"), "Customer services" },
                    { new Guid("796138b4-cc1c-4660-a6c9-18250cb20672"), "Business manager" },
                    { new Guid("92053e15-bb56-4170-9198-6f944d3b008a"), "Compliance officer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("796138b4-cc1c-4660-a6c9-18250cb20672"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("92053e15-bb56-4170-9198-6f944d3b008a"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ae25cb3d-b983-44eb-a233-430684f37330"));

            migrationBuilder.DropColumn(
                name: "IsProfileComplete",
                table: "Users");
        }
    }
}
