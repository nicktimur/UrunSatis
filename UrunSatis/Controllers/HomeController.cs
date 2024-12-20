using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using UrunSatis.Models;

namespace UrunSatis.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UrunSatisContext _db;
    private readonly IHubContext<UserHub> _hubContext;

    public HomeController(ILogger<HomeController> logger, UrunSatisContext db, IHubContext<UserHub> hubContext)
    {
        _logger = logger;
        _db = db;
        _hubContext = hubContext; //SignalR için
    }

    [SendUserInfo]
    public IActionResult Index()
    {
        var urunSayisi = _db.Uruns.Count();
        ViewBag.UrunSayisi = urunSayisi;

        var kullaniciSayisi = _db.Kullanicis.Count();
        ViewBag.KullaniciSayisi = kullaniciSayisi;

        // Son eklenen 5 ürünü al
        var sonUrunler = _db.Uruns
                        .OrderByDescending(g => g.Id)
                        .Take(5)
                        .Select(g => new
                        {
                            g.Id,
                            g.Baslik,
                            g.ResimYolu,
                            g.Kategori,
                            g.Aciklama,
                            g.Fiyat
                        }).ToList();

        // Son 5 ürünü ViewBag'e at
        ViewBag.sonUrunler = sonUrunler;

        return View();
    }

    [SendUserInfo]
    public IActionResult AboutUs()
    {
        return View();
    }

    [SendUserInfo]
    public IActionResult ContactUs()
    {
        return View();
    }

    [SendUserInfo]
    public IActionResult Urunler()
    {
        var urunler = _db.Uruns
                            .OrderByDescending(g => g.Id) // En büyük Id'den en küçüðe doðru sýrala
                            .Select(g => new Urun
                            {
                                Id = g.Id,
                                Baslik = g.Baslik,
                                ResimYolu = g.ResimYolu,
                                Kategori = g.Kategori,
                                Fiyat = g.Fiyat
                            }).ToList();
        ViewBag.Urunler = urunler;
        return View();
    }

    [SendUserInfo]
    public IActionResult Urun(int id)
    {
        var urun = _db.Uruns
           .FirstOrDefault(m => m.Id == id);
        _db.SaveChanges();
        return View(urun);
    }

    [SessionAuthorize(false)]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(KullaniciLoginModel kullanici)
    {
        if (kullanici is not null)
        {
            var user = _db.Kullanicis.Where(x => x.KullaniciAdi == kullanici.KullaniciAdi).FirstOrDefault();
            if (user is not null)
            {
                if (user.Sifre == AesEncryption.Encrypt(kullanici.Sifre))
                {
                    var userJson = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("user", userJson);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Hesap oluşturman gerekiyor olabilir.");
            }
        }
        return View();
    }

    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string EskiSifre { get; set; }
        public string YeniSifre { get; set; }
    }

    [HttpPost]
    public IActionResult ChangePassword([FromBody] ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Geçersiz veri gönderildi." });
        }

        // Kullanıcıyı ID ile bul
        var user = _db.Kullanicis.Where(x => x.Id == model.UserId).FirstOrDefault();
        if (user == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        // Eski şifre kontrolü
        var isOldPasswordCorrect = (model.EskiSifre == AesEncryption.Decrypt(user.Sifre));
        if (!isOldPasswordCorrect)
        {
            return Json(new { success = false, message = "Eski şifre yanlış." });
        }
        try
        {
            user.Sifre = AesEncryption.Encrypt(model.YeniSifre);
            _db.SaveChanges();
            return Json(new { success = true, message = "Şifre başarıyla değiştirildi." });
        }
        catch
        {
            return Json(new { success = false, message = "Şifre değiştirilemedi." });
        }
    }

    public class EditUserViewModel
    {
        public int UserId { get; set; }
        public string KullaniciAdi { get; set; }
        public string isim { get; set; }
        public string soyisim { get; set; }
    }

    [HttpPost]
    public IActionResult EditUser([FromBody] EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Geçersiz veri gönderildi." });
        }

        // Kullanıcıyı ID ile bul
        var user = _db.Kullanicis.Where(x => x.Id == model.UserId).FirstOrDefault();
        if (user == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        try
        {
            user.Isim = model.isim;
            user.Soyisim = model.soyisim;
            user.KullaniciAdi = model.KullaniciAdi;
            _db.SaveChanges();
            var userJson = HttpContext.Session.GetString("user");
            HttpContext.Session.Clear();
            userJson = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString("user", userJson);
            return Json(new { success = true, message = "Bilgiler başarıyla değiştirildi." });
        }
        catch
        {
            return Json(new { success = false, message = "Kullanıcı adı kullanılıyor olabilir." });
        }
    }


    [SessionAuthorize(false)]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(KullaniciAddingModel kullanici)
    {
        if (kullanici is not null)
        {
            Kullanici yeniKullanici = new Kullanici();
            yeniKullanici.Isim = kullanici.Isim;
            yeniKullanici.Soyisim = kullanici.Soyisim;
            yeniKullanici.Email = kullanici.Email;
            yeniKullanici.KullaniciAdi = kullanici.KullaniciAdi;
            yeniKullanici.Sifre = AesEncryption.Encrypt(kullanici.Sifre);
            yeniKullanici.AktifMi = true;
            yeniKullanici.KullaniciTipi = 0;
            try
            {
                _db.Kullanicis.Add(yeniKullanici);
                _db.SaveChanges();
                // SignalR ile kullanıcıların olduğu sayfayı bilgilendir
                await _hubContext.Clients.All.SendAsync("UserAdded", yeniKullanici);
            }
            catch
            {
                ViewBag.Error = "Bu mail veya kullanıcı adı zaten kullanılıyor";
                return View(yeniKullanici);
            }

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Error = "Girdiğiniz bilgilerde bir eksiklik bulunmakta";
            return View();
        }
    }

    public IActionResult Logout()
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null)
        {
            return RedirectToAction("Index", "Home");
        }
        var userdata = JsonConvert.DeserializeObject<Kullanici>(userJson);
        var user = _db.Kullanicis.Where(x => x.Email == userdata.Email).FirstOrDefault();
        _db.SaveChanges();
        HttpContext.Response.Cookies.Delete(".AspNetCore.Session", new CookieOptions
        {
            Expires = DateTime.Now.AddMinutes(-1),
            HttpOnly = true,
            Secure = true
        });
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
