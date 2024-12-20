using System;
using System.Collections.Generic;

namespace UrunSatis.Models;

public partial class UrunAddingModel
{
    public long? Id { get; set; }
    public string Baslik { get; set; }

    public string Kategori { get; set; }

    public decimal Fiyat {  get; set; }

    public IFormFile? Resim { get; set; }

    public string Aciklama { get; set; }
}
