using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeziBlogum.Migrations
{
    /// <inheritdoc />
    public partial class AddLikeDislikeToComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DislikeCount",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Comments");
        }
    }
}
