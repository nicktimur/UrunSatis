using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UrunSatis.Models;

namespace UrunSatis.Controllers
{
    public class AdminController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly UrunSatisContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(ILogger<HomeController> logger, UrunSatisContext db, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        [AdminOnly]
        [SendUserInfo]
        public IActionResult Index()
        {
            return View();
        }

        [SendUserInfo]
        [AdminOnly]
        public IActionResult Users()
        {
            var users = _db.Kullanicis.ToList();
            ViewBag.Users = JsonConvert.SerializeObject(users);
            return View();
        }


        [HttpPost]
        [SendUserInfo]
        [AdminOnly]
        public IActionResult Users(AdminUserEditViewModel kullanici)
        {
            var users = _db.Kullanicis.ToList();
            ViewBag.Users = JsonConvert.SerializeObject(users);
            var user = _db.Kullanicis.Where(x => x.Id == kullanici.Id).FirstOrDefault();
            if (user is not null)
            {
                try
                {
                    user.KullaniciAdi = kullanici.KullaniciAdi ?? user.KullaniciAdi;
                    user.Isim = kullanici.Isim ?? user.Isim;
                    user.Soyisim = kullanici.Soyisim ?? user.Soyisim;
                    user.Email = kullanici.Email ?? user.Email;
                    user.KullaniciTipi = kullanici.KullaniciTipi ?? user.KullaniciTipi;
                    user.GuncellemeTarihi = DateTime.Now;

                    _db.SaveChanges();

                    users = _db.Kullanicis.ToList();
                    ViewBag.Users = JsonConvert.SerializeObject(users);
                    var userJson = HttpContext.Session.GetString("user");
                    var userdata = JsonConvert.DeserializeObject<Kullanici>(userJson);
                    var currentUser = _db.Kullanicis.Where(x => x.KullaniciAdi == userdata.KullaniciAdi).FirstOrDefault();
                    if (currentUser.Id == user.Id)
                    {
                        HttpContext.Session.Clear();
                        userJson = JsonConvert.SerializeObject(user);
                        HttpContext.Session.SetString("user", userJson);
                    }
                    return View(user);
                }
                catch
                {
                    ViewBag.Error = "Lütfen kullanılmayan bir mail veya kullanıcı adı giriniz.";
                    return View();
                }
            }
            return View();

        }

        [HttpGet("GetUserInfo/{id}")]
        public IActionResult GetUserInfo(int id)
        {
            var user = _db.Kullanicis.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            return Json(user);
        }

        [AdminOnly]
        [SendUserInfo]
        [HttpPost]
        public IActionResult DeleteUser(AdminUserEditViewModel kullanici)
        {
            var user = _db.Kullanicis.Where(u => u.Id == kullanici.Id).FirstOrDefault();
            _db.Kullanicis.Remove(user);
            _db.SaveChanges();
            var userJson = HttpContext.Session.GetString("user");
            var userdata = JsonConvert.DeserializeObject<Kullanici>(userJson);
            var currentUser = _db.Kullanicis.Where(x => x.KullaniciAdi == userdata.KullaniciAdi).FirstOrDefault();
            if (currentUser.Id == user.Id)
            {
                HttpContext.Session.Clear();
                userJson = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString("user", userJson);
            }

            return RedirectToAction("Users", "Admin");
        }


        [SendUserInfo]
        [AdminOnly]
        public IActionResult AddUrun()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUrun(UrunAddingModel urun)
        {
            if (ModelState.IsValid)
            {
                var fileName = "";
                // Resim dosyası kontrolü
                if (urun.Resim != null && urun.Resim.Length > 0)
                {
                    // Dosya adını ve yolunu belirleyin
                    fileName = $"{urun.Baslik.Replace(" ", "_")}_{Guid.NewGuid()}{Path.GetExtension(urun.Resim.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/UrunGorselleri", fileName);

                    // Resim dosyasını kaydedin
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await urun.Resim.CopyToAsync(stream);
                    }
                }
                else
                {
                    fileName = "default.jpg";
                }

                // Yeni ürün nesnesi oluşturun
                Urun yeniUrun = new Urun
                {
                    Baslik = urun.Baslik,
                    Aciklama = await SaveImagesAndUpdatePaths(urun.Aciklama, "img/UrunGorselleri"),
                    Kategori = urun.Kategori,
                    Fiyat = urun.Fiyat,
                    ResimYolu = $"/img/UrunGorselleri/{fileName}"
                };

                _db.Uruns.Add(yeniUrun);
                await _db.SaveChangesAsync();
                return View();
            }
            return View();
        }

        private async Task<string> SaveImagesAndUpdatePaths(string htmlContent, string adres)
        {
            string imageFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, adres);
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }

            string pattern = "<img[^>]+src=\"data:image/(?<type>.+?);base64,(?<data>.+?)\"[^>]*>";
            var matches = Regex.Matches(htmlContent, pattern);

            foreach (Match match in matches)
            {
                var base64Data = match.Groups["data"].Value;
                var imageType = match.Groups["type"].Value;
                var imageData = Convert.FromBase64String(base64Data);
                var imageName = $"{Guid.NewGuid()}.{imageType}";
                var imagePath = Path.Combine(imageFolderPath, imageName);

                await System.IO.File.WriteAllBytesAsync(imagePath, imageData);

                // Güncellenmiş path ile src'yi değiştirin
                var newSrc = "/" + adres + $"/{imageName}";
                htmlContent = htmlContent.Replace(match.Groups[0].Value, $"<img class=\"img-fluid\" src=\"{newSrc}\" />");
            }

            return htmlContent;
        }

        [HttpPost]
        public IActionResult RegisterAdmin(KullaniciAddingModel kullanici)
        {
            if (kullanici is not null)
            {
                Kullanici yeniKullanici = new Kullanici();
                yeniKullanici.Isim = kullanici.Isim;
                yeniKullanici.Soyisim = kullanici.Soyisim;
                yeniKullanici.Email = kullanici.Email;
                yeniKullanici.KullaniciAdi = kullanici.KullaniciAdi;
                yeniKullanici.AktifMi = true;
                yeniKullanici.Sifre = AesEncryption.Encrypt(kullanici.Sifre);
                yeniKullanici.KullaniciTipi = kullanici.KullaniciTipi;

                _db.Kullanicis.Add(yeniKullanici);
                _db.SaveChanges();


                return RedirectToAction("Users", "Admin");
            }
            else
                return RedirectToAction("Users", "Admin");
        }

    }
}
