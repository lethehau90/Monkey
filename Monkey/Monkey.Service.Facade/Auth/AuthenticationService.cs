﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey → Service Facade </Project>
//     <File>
//         <Name> AuthenticationService.cs </Name>
//         <Created> 13/09/17 10:48:09 PM </Created>
//         <Key> d4fd8acd-6479-492d-bcae-5e0293049dbe </Key>
//     </File>
//     <Summary>
//         AuthenticationService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using Monkey.Auth;
using Monkey.Auth.Helpers;
using Monkey.Auth.Interfaces;
using Monkey.Business.Auth;
using Monkey.Core;
using Monkey.Core.Constants.Auth;
using Monkey.Core.Models.Auth;
using Puppy.DependencyInjection.Attributes;
using System.Threading.Tasks;
using HttpContext = System.Web.HttpContext;

namespace Monkey.Service.Facade.Auth
{
    [PerRequestDependency(ServiceType = typeof(IAuthenticationService))]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationBusiness _authenticationBusiness;
        private readonly IClientBusiness _clientBusiness;

        public AuthenticationService(IAuthenticationBusiness authenticationBusiness, IClientBusiness clientBusiness)
        {
            _authenticationBusiness = authenticationBusiness;
            _clientBusiness = clientBusiness;
        }

        /// <inheritdoc />
        public async Task<AccessTokenModel> SignInAsync(RequestTokenModel model)
        {
            int? clientId = null;

            // Case Client Id and Client Secret is null is system sign in => by pass validate
            if (!string.IsNullOrWhiteSpace(model.ClientId) || !string.IsNullOrWhiteSpace(model.ClientSecret))
            {
                _clientBusiness.CheckExist(model.ClientId, model.ClientSecret);

                _clientBusiness.CheckBanned(model.ClientId, model.ClientSecret);

                clientId = await _clientBusiness.GetIdAsync(model.ClientId, model.ClientSecret).ConfigureAwait(true);
            }

            AccessTokenModel accessTokenModel = null;

            if (model.GrantType == GrantType.Password)
            {
                _authenticationBusiness.CheckExistByUserName(model.UserName);

                _authenticationBusiness.CheckValidSignIn(model.UserName, model.Password);

                LoggedInUser.Current = _authenticationBusiness.SignIn(model.UserName, model.Password, out string refreshToken, clientId);

                // Generate access token
                accessTokenModel = TokenHelper.GenerateAccessToken(model.ClientId, LoggedInUser.Current.Subject, AuthConfig.AccessTokenExpireIn, refreshToken);

                HttpContext.Current.User = TokenHelper.GetClaimsPrincipal(accessTokenModel.AccessToken);
            }
            else if (model.GrantType == GrantType.RefreshToken)
            {
                _authenticationBusiness.CheckValidRefreshToken(model.RefreshToken, clientId);

                LoggedInUser.Current = await _authenticationBusiness.GetLoggedInUserByRefreshTokenAsync(model.RefreshToken).ConfigureAwait(true);

                // Generate access token
                accessTokenModel = TokenHelper.GenerateAccessToken(model.ClientId, LoggedInUser.Current.Subject, AuthConfig.AccessTokenExpireIn, model.RefreshToken);

                HttpContext.Current.User = TokenHelper.GetClaimsPrincipal(accessTokenModel.AccessToken);
            }

            return accessTokenModel;
        }

        /// <inheritdoc />
        public async Task SignInCookieAsync(IResponseCookies cookies, AccessTokenModel accessTokenModel)
        {
            TokenHelper.SetAccessTokenInCookie(cookies, accessTokenModel);

            LoggedInUser.Current = await GetLoggedInUserAsync(accessTokenModel.AccessToken).ConfigureAwait(true);

            HttpContext.Current.User = TokenHelper.GetClaimsPrincipal(accessTokenModel.AccessToken);
        }

        /// <inheritdoc />
        public async Task<AccessTokenModel> SignInCookieAsync(IRequestCookieCollection cookies)
        {
            var accessTokenModel = TokenHelper.GetAccessTokenInCookie(cookies);

            if (accessTokenModel == null)
            {
                return null;
            }

            string accessTokenClientId = TokenHelper.GetAccessTokenClientId(accessTokenModel.AccessToken);

            if (!TokenHelper.IsValidToken(accessTokenModel.AccessToken) || accessTokenClientId != null)
            {
                return null;
            }

            LoggedInUser.Current = await GetLoggedInUserAsync(accessTokenModel.AccessToken).ConfigureAwait(true);

            HttpContext.Current.User = TokenHelper.GetClaimsPrincipal(accessTokenModel.AccessToken);

            return accessTokenModel;
        }

        /// <inheritdoc />
        public Task SignOutCookieAsync(IResponseCookies cookies)
        {
            TokenHelper.RemoveAccessTokenInCookie(cookies);

            LoggedInUser.Current = null;

            if (HttpContext.Current != null && HttpContext.Current.User != null)
            {
                HttpContext.Current.User = null;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<LoggedInUserModel> GetLoggedInUserAsync(string accessToken)
        {
            string subject = TokenHelper.GetAccessTokenSubject(accessToken);
            _authenticationBusiness.CheckExistsBySubject(subject);
            return _authenticationBusiness.GetLoggedInUserBySubjectAsync(subject);
        }

        /// <inheritdoc />
        public Task ExpireAllRefreshTokenAsync(string accessToken)
        {
            string subject = TokenHelper.GetAccessTokenSubject(accessToken);
            _authenticationBusiness.CheckExistsBySubject(subject);
            return _authenticationBusiness.ExpireAllRefreshTokenAsync(subject);
        }
    }
}