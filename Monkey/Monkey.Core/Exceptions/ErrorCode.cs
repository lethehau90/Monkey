﻿#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Monkey.Auth.Domain.Exceptions </Project>
//     <File>
//         <Name> ExceptionCode </Name>
//         <Created> 12/04/2017 09:05:31 AM </Created>
//         <Key> f183ec48-2edb-4c90-b029-6ee78f6b6cdd </Key>
//     </File>
//     <Summary>
//         ExceptionCode
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Monkey.Core.Exceptions
{
    /// <summary>
    ///     Error code for whole system, [Display(Group Name)] for Module, [Description] for default message
    /// </summary>
    public enum ErrorCode
    {
        // Global

        [Description("Bad Request")]
        [Display(GroupName = "Global")]
        BadRequest = 400,

        [Display(GroupName = "Global")]
        [Description("Un-Authenticate")]
        UnAuthenticated = 401,

        [Display(GroupName = "Global")]
        [Description("Forbidden")]
        UnAuthorized = 403,

        [Display(GroupName = "Global")]
        [Description("Not Found, the resource does not exist")]
        NotFound = 404,

        [Description("Oops! Something went wrong, please try again later")]
        [Display(GroupName = "Global")]
        Unknown = 500,

        // Access Token

        [Display(GroupName = "Token")]
        [Description("Invalid refresh token")]
        InvalidRefreshToken = 600,

        [Display(GroupName = "Token")]
        [Description("Refresh token is expired")]
        RefreshTokenExpired = 601,

        [Display(GroupName = "Token")]
        [Description("Invalid access token")]
        InvalidAccessToken = 602,

        [Display(GroupName = "Token")]
        [Description("Access token is expired")]
        AccessTokenExpired = 603,

        // Client

        [Display(GroupName = "Client")]
        [Description("Invalid client, wrong client id or client secret")]
        InvalidClient = 700,

        [Display(GroupName = "Client")]
        [Description("Client is banned")]
        ClientIsBanned = 701,

        [Display(GroupName = "Client")]
        [Description("Client name is already exist, please try another")]
        ClientNameNotUnique = 703,

        // User

        [Display(GroupName = "User")]
        [Description("User is in-active")]
        UserInActive = 1000,

        [Display(GroupName = "User")]
        [Description("User Name is already exist")]
        UserNameNotUnique = 1001,

        [Display(GroupName = "User")]
        [Description("User password is wrong")]
        UserPasswordWrong = 1002,

        [Display(GroupName = "User")]
        [Description("User is banned")]
        UserBanned = 1003,

        [Display(GroupName = "User")]
        [Description("Email is already exist")]
        UserEmailNotUnique = 1004,

        [Display(GroupName = "User")]
        [Description("User can't self update")]
        UserSelfUpdate = 1005,

        [Display(GroupName = "User")]
        [Description("User can't self remove")]
        UserSelfRemove = 1006,

        [Display(GroupName = "User")]
        [Description("Can't remove this user")]
        UserCannotRemove = 1007,

        [Display(GroupName = "User")]
        [Description("Can't update this user")]
        UserCannotUpdate = 1008,

        [Display(GroupName = "User")]
        [Description("Phone is already exist, please try another")]
        UserPhoneNotUnique = 1009,

        [Display(GroupName = "User")]
        [Description("Confirm email token expired or invalid")]
        UserConfirmEmailTokenExpireOrInvalid = 1010,

        [Display(GroupName = "User")]
        [Description("Set password token expired or invalid")]
        UserSetPasswordTokenExpireOrInvalid = 1011,
    }
}