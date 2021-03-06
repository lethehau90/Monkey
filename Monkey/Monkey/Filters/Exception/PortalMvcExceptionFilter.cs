﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Monkey.Extensions;
using Puppy.Logger;
using Puppy.Web;
using Puppy.Web.HttpUtils;
using System;

namespace Monkey.Filters.Exception
{
    public class PortalMvcExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public PortalMvcExceptionFilter(ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        public override void OnException(ExceptionContext context)
        {
            var errorModel = ExceptionContextHelper.GetErrorModel(context);

            // Ajax Case
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new JsonResult(errorModel, Puppy.Core.Constants.StandardFormat.JsonSerializerSettings);

                context.ExceptionHandled = true;

                // Keep base Exception
                base.OnException(context);

                return;
            }

            // MVC Page
            if (context.Exception is UnauthorizedAccessException)
            {
                Log.Error(context);

                // Redirect to un-authorization page
                context.Result = new RedirectToActionResult("Index", "Auth", new { area = "Portal" }, false);
            }
            else
            {
                Log.Fatal(context);
#if DEBUG
                // Keep base Exception
                base.OnException(context);
                return;
#else
// Redirect to Oops page
                context.Result = new RedirectToActionResult("Index", "Auth", new { area = "Portal" }, false);
#endif
            }

            // Notify
            var tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);

            tempData.Set(Constants.TempDataKey.Notify,
                new NotifyResultViewModel
                {
                    Title = "Oops !",
                    Message = errorModel.Message,
                    Status = NotifyStatus.Error
                });

            context.ExceptionHandled = true;

            // Keep base Exception
            base.OnException(context);
        }
    }
}