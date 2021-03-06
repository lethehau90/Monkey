﻿using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc;
using Monkey.Core.Validators.Auth;
using System.ComponentModel.DataAnnotations;

namespace Monkey.Core.Models.Auth
{
    [Validator(typeof(CannotSigInModelValidator))]
    public class CannotSigInModel
    {
        [DataType(DataType.EmailAddress)]
        [Remote("CheckExistEmail", "User", HttpMethod = "POST", ErrorMessage = "The email does not exist.")]
        public string Email { get; set; }
    }
}