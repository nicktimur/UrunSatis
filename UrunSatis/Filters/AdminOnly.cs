using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using UrunSatis.Models;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AdminOnly : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Session.GetString("user") != null)
        {
            var userJson = context.HttpContext.Session.GetString("user");
            var userData = JsonConvert.DeserializeObject<Kullanici>(userJson);
            if (userData.KullaniciTipi == 0) //Admin değilse yönlendir
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
        else //Kullanıcı yoksa yönlendir
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}