using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FestivalFlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.AddColumn<string>(
    name: "Data",
    table: "Request",
    type: "nvarchar(max)",
    nullable: true);
        }

    
       
        }
    
}
