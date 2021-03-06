﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> MvcApiExtensions.cs </Name>
//         <Created> 31/07/17 10:42:40 PM </Created>
//         <Key> f0c4a37a-1d5d-422e-885f-38e3acce572e </Key>
//     </File>
//     <Summary>
//         MvcApiExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Monkey.Binders;
using Monkey.Controllers;
using Monkey.Core.Configs;
using Monkey.Core.Localization;
using Monkey.Core.Validators;
using Monkey.Filters.Exception;
using Monkey.Filters.ModelValidation;
using Puppy.Core.EnvironmentUtils;
using Puppy.Core.StringUtils;
using Puppy.DataTable;
using Puppy.Web.Constants;
using Puppy.Web.HttpUtils;
using Puppy.Web.Middlewares;
using Puppy.Web.Render;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Monkey.Extensions
{
    public static class MvcApiExtensions
    {
        /// <summary>
        ///     [Mvc - API] Json, Xml serialize, area, response caching and filters 
        /// </summary>
        /// <param name="services">         </param>
        /// <param name="configurationRoot"></param>
        public static IServiceCollection AddMvcApi(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            if (!EnvironmentHelper.IsDevelopment())
            {
                services
                    .AddResponseCaching()

                    // [Mini Response]
                    .AddMinResponse();
            }

            services
                // Api Filter
                .AddScoped<ApiExceptionFilter>()
                .AddScoped<ApiModelValidationActionFilter>()

                // Mvc Services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<IViewRenderService, ViewRenderService>()

                // Mvc Filter
                .AddScoped<PortalMvcExceptionFilter>()
                .AddScoped<AjaxModelValidationActionFilter>()

                // Enable Session to use TempData
                .AddSingleton<ITempDataProvider, CookieTempDataProvider>()

                // [MVC] Anti Forgery
                //.AddAntiforgeryToken()

                // [DataTable]
                .AddDataTable(configurationRoot, typeof(SharedResources))

                // [Binders]
                .AddDateTimeOffsetBinder()

                // [Localizer]
                .AddLocalization()
                .Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture(Core.Constants.Culture.Vietnamese);
                    options.SupportedCultures = options.SupportedUICultures = new List<CultureInfo>
                    {
                        new CultureInfo(Core.Constants.Culture.Vietnamese),
                        new CultureInfo(Core.Constants.Culture.English)
                    };
                })

                // Area
                .Configure<RazorViewEngineOptions>(options =>
                {
                    options.AreaViewLocationFormats.Clear();
                    options.AreaViewLocationFormats.Add("/" + SystemConfig.MvcPath.AreasRootFolderName + "/{2}/Views/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                })

                // Setup Mvc
                .AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = false; // false by default
                    options.Filters.Add(new ProducesAttribute(ContentType.Json));
                    options.Filters.Add(new ProducesAttribute(ContentType.Xml));
                })
                .AddXmlDataContractSerializerFormatters()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Puppy.Core.Constants.StandardFormat.JsonSerializerSettings.ReferenceLoopHandling;
                    options.SerializerSettings.NullValueHandling = Puppy.Core.Constants.StandardFormat.JsonSerializerSettings.NullValueHandling;
                    options.SerializerSettings.Formatting = Puppy.Core.Constants.StandardFormat.JsonSerializerSettings.Formatting;
                    options.SerializerSettings.ContractResolver = Puppy.Core.Constants.StandardFormat.JsonSerializerSettings.ContractResolver;
                    options.SerializerSettings.DateTimeZoneHandling = Puppy.Core.Constants.StandardFormat.JsonSerializerSettings.DateTimeZoneHandling;
                    options.SerializerSettings.DateFormatString = SystemConfig.SystemDateTimeFormat;
                })

                // [Validator] Model Validator, Must after "AddMvc"
                .AddModelValidator()

                // [Localizer]
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
                });

            return services;
        }

        /// <summary>
        ///     [Mvc - API] Static files configuration, routing [Mvc] Static files configuration, routing
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseMvcApi(this IApplicationBuilder app)
        {
            if (!EnvironmentHelper.IsDevelopment())
            {
                app
                    .UseResponseCaching()

                    // [Mini Response]
                    .UseMinResponse();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app
                // [DataTable]
                .UseDataTable()

                // Status Code Handle
                .UseStatusCodePages(context =>
                {
                    string requestPath = context.HttpContext.Request.Path.Value.Trim('/');

                    // Handle static resource in advance

                    var staticResourceRelativeUrl = SystemConfig.MvcPath.GetStaticFilesRelativeUrl();

                    List<string> listStaticResourceExtension = new List<string>
                    {
                        ".css",
                        ".scss",
                        ".less",
                        ".js",
                        ".js.map",
                        ".json",
                        ".rss",
                        ".xml",
                        ".mp3",
                        ".mp4",
                        ".ogg",
                        ".ogv",
                        ".webm",
                        ".svg",
                        ".svgz",
                        ".eot",
                        ".tff",
                        ".otf",
                        ".woff",
                        ".woff2",
                        ".crx",
                        ".xpi",
                        ".safariextz",
                        ".flv",
                        ".f4v",
                        ".png",
                        ".jpeg",
                        ".jpg",
                        ".bmp"
                    };

                    bool isRequestForExistingStaticResource =
                        context.HttpContext.Request.Method.Equals(HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase)
                        && staticResourceRelativeUrl.Any(x => context.HttpContext.Request.IsRequestFor(x));

                    bool isRequestForStaticResourceByExtension =
                        context.HttpContext.Request.Method.Equals(HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase)
                        && listStaticResourceExtension.Any(x => requestPath.EndsWith(x, StringComparison.OrdinalIgnoreCase));

                    string apiAreaRootPath = Areas.Api.Controllers.ApiController.AreaName.Trim('/');

                    string portalAreaRootPath = Areas.Portal.Controllers.MvcController.AreaName.Trim('/');

                    if (requestPath.StartsWith(apiAreaRootPath))
                    {
                        // Api Area

                        // Don't handle
                    }
                    else if (requestPath.StartsWith(portalAreaRootPath))
                    {
                        // Portal Area

                        if (isRequestForExistingStaticResource || isRequestForStaticResourceByExtension)
                        {
                            context.HttpContext.Response.ContentType = $"{ContentType.Html}; charset=UTF-8";

                            context.HttpContext.Response.WriteAsync($@"Not Found <br /><a href='{context.HttpContext.Request.GetDomain()}/{portalAreaRootPath}'>Back to Eat Up.</a>");

                            return Task.CompletedTask;
                        }

                        // Redirect to error page
                        context.HttpContext.Response.Redirect($"{context.HttpContext.Request.GetDomain()}/{portalAreaRootPath}/{Areas.Portal.Controllers.HomeController.OopsEndpoint}/{context.HttpContext.Response.StatusCode}");
                    }
                    else
                    {
                        // Root

                        if (isRequestForExistingStaticResource || isRequestForStaticResourceByExtension)
                        {
                            context.HttpContext.Response.ContentType = $"{ContentType.Html}; charset=UTF-8";

                            context.HttpContext.Response.WriteAsync($@"Not Found <br /><a href='{context.HttpContext.Request.GetDomain()}'>Back to Eat Up.</a>");

                            return Task.CompletedTask;
                        }

                        // Redirect to error page
                        context.HttpContext.Response.Redirect($"{context.HttpContext.Request.GetDomain()}/{HomeController.OopsEndpoint}/{context.HttpContext.Response.StatusCode}");
                    }

                    return Task.CompletedTask;
                })

                // Root Path and GZip
                .UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = (context) =>
                    {
                        var headers = context.Context.Response.GetTypedHeaders();
                        headers.CacheControl = new CacheControlHeaderValue
                        {
                            MaxAge = SystemConfig.MvcPath.MaxAgeResponseHeader
                        };
                    }
                });

            // Path and GZip for Statics Content
            if (SystemConfig.MvcPath.StaticsContents?.Any() == true)
            {
                foreach (var staticsContent in SystemConfig.MvcPath.StaticsContents)
                {
                    string fileProviderPath = Path.Combine(staticsContent.Area, staticsContent.FolderRelativePath);

                    fileProviderPath =
                        string.IsNullOrWhiteSpace(SystemConfig.MvcPath.AreasRootFolderName)
                            ? fileProviderPath
                            : Path.Combine(SystemConfig.MvcPath.AreasRootFolderName, fileProviderPath);

                    fileProviderPath = fileProviderPath.GetFullPath();

                    // Skip if Directory is not exists
                    if (!Directory.Exists(fileProviderPath))
                    {
                        continue;
                    }

                    PhysicalFileProvider fileProvider = new PhysicalFileProvider(fileProviderPath);

                    PathString requestPath = new PathString(staticsContent.HttpRequestPath);

                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = fileProvider,
                        RequestPath = requestPath,
                        OnPrepareResponse = (context) =>
                        {
                            var headers = context.Context.Response.GetTypedHeaders();
                            headers.CacheControl = new CacheControlHeaderValue
                            {
                                MaxAge = staticsContent.MaxAgeResponseHeader
                            };
                        }
                    });
                }
            }
            // [Localizer]
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;

            // ADD Cookie
            var cookieProvider = localizationOptions.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First();
            cookieProvider.CookieName = Core.Constants.Culture.CookieName;

            // REMOVE Accept-Language
            var acceptLanguageHeaderProvider = localizationOptions.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
            localizationOptions.RequestCultureProviders.Remove(acceptLanguageHeaderProvider);

            app.UseRequestLocalization(localizationOptions);

            // Config Global Route
            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}");
            });

            return app;
        }

        /// <summary>
        ///     Configures the anti-forgery tokens for better security. 
        /// </summary>
        /// <param name="services"></param>
        /// <remarks> See: http://www.asp.net/mvc/overview/security/xsrfcsrf-prevention-in-aspnet-mvc-and-web-pages </remarks>
        public static IServiceCollection AddAntiforgeryToken(this IServiceCollection services)
        {
            services.AddAntiforgery(
                options =>
                {
                    // Rename the Anti-Forgery cookie from "__RequestVerificationToken" to "ape".
                    // This adds a little security through obscurity and also saves sending a few
                    // characters over the wire.
                    options.Cookie.Name = "ape";

                    // Rename the form input name from "__RequestVerificationToken" to "ape" for the
                    // same reason above e.g.
                    // <input name="__RequestVerificationToken" type="hidden" value="..." />
                    options.FormFieldName = "ape";

                    // Rename the Anti-Forgery HTTP header from RequestVerificationToken to
                    // X-XSRF-TOKEN. X-XSRF-TOKEN is not a standard but a common name given to this
                    // HTTP header popularized by Angular.
                    options.HeaderName = HeaderKey.XAntiforgeryToken;

                    // If you have enabled SSL/TLS. Uncomment this line to ensure that the
                    // Anti-Forgery cookie requires SSL /TLS to be sent across the wire.
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                });

            return services;
        }
    }
}