using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrunSatis.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kullanici",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    soyisim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    kullanici_adi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sifre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    kullanici_tipi = table.Column<int>(type: "int", nullable: false),
                    aktif_mi = table.Column<bool>(type: "bit", nullable: false),
                    olusturulma_tarihi = table.Column<byte[]>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    silinme_tarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    guncelleme_tarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    son_aktif_tarih = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kullanici", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "urun",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    baslik = table.Column<string>(type: "text", nullable: false),
                    aciklama = table.Column<string>(type: "text", nullable: false),
                    fiyat = table.Column<long>(type: "bigint", nullable: false),
                    StokMiktar = table.Column<int>(type: "int", nullable: false),
                    kategori = table.Column<string>(type: "text", nullable: false),
                    resim_yolu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    olusturulma_tarihi = table.Column<byte[]>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    aktif_mi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_urun", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "email",
                table: "kullanici",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "kullanici_adi",
                table: "kullanici",
                column: "kullanici_adi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kullanici");

            migrationBuilder.DropTable(
                name: "urun");
        }
    }
}
