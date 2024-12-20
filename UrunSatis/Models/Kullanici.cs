using System;
using System.Collections.Generic;

namespace UrunSatis.Models;

public partial class Kullanici
{
    public long Id { get; set; }

    public string? Isim { get; set; }

    public string? Soyisim { get; set; }

    public string KullaniciAdi { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Sifre { get; set; } = null!;

    public int KullaniciTipi { get; set; }

    public bool AktifMi { get; set; }

    public DateTime OlusturulmaTarihi { get; set; }

    public DateTime? SilinmeTarihi { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public DateTime? SonAktifTarih { get; set; }
}