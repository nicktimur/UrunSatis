using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly bool _canGoLoggedIn;

    public SessionAuthorizeAttribute(bool canGoLoggedIn)
    {
        _canGoLoggedIn = canGoLoggedIn;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!_canGoLoggedIn && context.HttpContext.Session.GetString("user") != null) //Sadece giriş yapmışken girilmezse ve kullanıcı null değilse
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
        else if (_canGoLoggedIn && context.HttpContext.Session.GetString("user") == null) //Sadece giriş yapmışken girebilirse ve kullanıcı yoksa
        {
            context.Result = new RedirectToActionResult("Index", "Admin", null);
        }
    }
}