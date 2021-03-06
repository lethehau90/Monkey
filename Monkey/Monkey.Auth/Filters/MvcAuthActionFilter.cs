﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> MvcAuthActionFilter.cs </Name>
//         <Created> 06/10/17 11:51:19 PM </Created>
//         <Key> 39f29144-7e98-40f8-97cc-83907fc6648b </Key>
//     </File>
//     <Summary>
//         MvcAuthActionFilter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Puppy.Web.HttpUtils;

namespace Monkey.Auth.Filters
{
    public class MvcAuthActionFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                if (!context.IsAuthenticated())
                {
                    context.Result = new JsonResult(new { });
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (!context.IsAuthorized())
                {
                    context.Result = new JsonResult(new { });
                    context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

                return;
            }

            if (!context.IsAuthenticated())
            {
                var redirectUrl = context.HttpContext.Request.GetDisplayUrl();
                context.Result = new RedirectToActionResult("Index", "Auth", new { area = "Portal", RedirectUrl = redirectUrl }, false);
                return;
            }

            if (!context.IsAuthorized())
            {
                context.Result = new RedirectToActionResult("Index", "Auth", new { area = "Portal" }, false);
            }
        }
    }
}