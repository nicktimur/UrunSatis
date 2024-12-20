namespace UrunSatis.Models
{
    public class Urun
    {
        public int Id { get; set; }

        public string Baslik { get; set; }

        public string Aciklama { get; set; }

        public decimal Fiyat { get; set; }

        public int StokMiktar { get; set; }

        public string Kategori { get; set; }

        public string ResimYolu { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public bool AktifMi { get; set; } = true;
    }
}
