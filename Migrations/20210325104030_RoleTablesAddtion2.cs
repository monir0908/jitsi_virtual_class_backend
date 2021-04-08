using Microsoft.EntityFrameworkCore.Migrations;

namespace Commander.Migrations
{
    public partial class RoleTablesAddtion2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HeadRoles_Roles_HeadRoles_HeadRolesId",
                table: "HeadRoles_Roles");

            migrationBuilder.DropIndex(
                name: "IX_HeadRoles_Roles_HeadRolesId",
                table: "HeadRoles_Roles");

            migrationBuilder.DropColumn(
                name: "HeadRolesId",
                table: "HeadRoles_Roles");

            migrationBuilder.CreateIndex(
                name: "IX_HeadRoles_Roles_HeadRoleId",
                table: "HeadRoles_Roles",
                column: "HeadRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_HeadRoles_Roles_HeadRoles_HeadRoleId",
                table: "HeadRoles_Roles",
                column: "HeadRoleId",
                principalTable: "HeadRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HeadRoles_Roles_HeadRoles_HeadRoleId",
                table: "HeadRoles_Roles");

            migrationBuilder.DropIndex(
                name: "IX_HeadRoles_Roles_HeadRoleId",
                table: "HeadRoles_Roles");

            migrationBuilder.AddColumn<long>(
                name: "HeadRolesId",
                table: "HeadRoles_Roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeadRoles_Roles_HeadRolesId",
                table: "HeadRoles_Roles",
                column: "HeadRolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_HeadRoles_Roles_HeadRoles_HeadRolesId",
                table: "HeadRoles_Roles",
                column: "HeadRolesId",
                principalTable: "HeadRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
