using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace UrunSatis.Models
{
    public partial class UrunSatisContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public UrunSatisContext(DbContextOptions<UrunSatisContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Urun> Uruns { get; set; }

        public virtual DbSet<Kullanici> Kullanicis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // appsettings.json'dan bağlantı dizesini al
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("Turkish_CI_AS");

            modelBuilder.Entity<Urun>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("urun");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .HasColumnName("id");
                entity.Property(e => e.Baslik)
                .HasColumnType("text")
                .HasColumnName("baslik");
                entity.Property(e => e.Aciklama)
                .HasColumnType("text")
                .HasColumnName("aciklama");
                entity.Property(e => e.Kategori)
                .HasColumnType("text")
                .HasColumnName("kategori");
                entity.Property(e => e.Fiyat)
                    .HasColumnType("bigint")
                    .HasColumnName("fiyat");
                entity.Property(e => e.ResimYolu)
                    .HasMaxLength(255)
                    .HasColumnName("resim_yolu");
                entity.Property(e => e.OlusturulmaTarihi)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp")
                    .HasColumnName("olusturulma_tarihi");
                entity.Property(e => e.AktifMi).HasColumnName("aktif_mi");
            });

            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("kullanici");

                entity.HasIndex(e => e.Email, "email").IsUnique();

                entity.HasIndex(e => e.KullaniciAdi, "kullanici_adi").IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.GuncellemeTarihi)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("datetime")
                    .HasColumnName("guncelleme_tarihi");

                entity.Property(e => e.Isim)
                    .HasMaxLength(255)
                    .HasColumnName("isim");

                entity.Property(e => e.KullaniciAdi)
                    .HasColumnName("kullanici_adi");

                entity.Property(e => e.KullaniciTipi)
                    .HasColumnType("int")
                    .HasColumnName("kullanici_tipi");

                entity.Property(e => e.OlusturulmaTarihi)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp")
                    .HasColumnName("olusturulma_tarihi");

                entity.Property(e => e.Sifre)
                    .HasMaxLength(255)
                    .HasColumnName("sifre");

                entity.Property(e => e.SilinmeTarihi)
                    .HasColumnType("datetime")  
                    .HasColumnName("silinme_tarihi");

                entity.Property(e => e.SonAktifTarih)
                    .HasColumnType("datetime")
                    .HasColumnName("son_aktif_tarih");

                entity.Property(e => e.Soyisim)
                    .HasMaxLength(255)
                    .HasColumnName("soyisim");

                entity.Property(e => e.AktifMi)
                    .HasColumnName("aktif_mi");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
