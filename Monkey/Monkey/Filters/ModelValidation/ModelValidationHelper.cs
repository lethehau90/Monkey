﻿using Microsoft.AspNetCore.Mvc.Filters;
using Puppy.Web;
using System.Collections.Generic;
using System.Linq;

namespace Monkey.Filters.ModelValidation
{
    public static class ModelValidationHelper
    {
        public static Dictionary<string, object> GetModelStateInvalidInfo(ActionExecutingContext context)
        {
            var keyValueInValid = new Dictionary<string, object> { { "Url", context.HttpContext.Request.GetDisplayUrl() } };
            foreach (var keyValueState in context.ModelState)
            {
                var error = string.Join(", ", keyValueState.Value.Errors.Select(x => x.ErrorMessage));
                keyValueInValid.Add(keyValueState.Key, error);
            }
            return keyValueInValid;
        }
    }
}