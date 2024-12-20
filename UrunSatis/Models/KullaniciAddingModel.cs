namespace UrunSatis.Models
{
    public class KullaniciAddingModel
    {
        public string? Isim { get; set; }

        public string? Soyisim { get; set; }

        public string KullaniciAdi { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Sifre { get; set; } = null!;

        public int KullaniciTipi { get; set; }
    }
}
