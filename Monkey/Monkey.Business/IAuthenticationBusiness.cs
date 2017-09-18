﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey → Business Interface </Project>
//     <File>
//         <Name> IAuthenticationBusiness.cs </Name>
//         <Created> 13/09/17 10:47:20 PM </Created>
//         <Key> 0b6435fb-3d3a-4fe1-ae17-a9703a45d61f </Key>
//     </File>
//     <Summary>
//         IAuthenticationBusiness.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Monkey.Core.Models.Auth;
using System.Threading.Tasks;

namespace Monkey.Business
{
    public interface IAuthenticationBusiness : IBaseBusiness
    {
        /// <summary>
        ///     Check user name and password is correct, not banned and already active 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        void CheckValidSignIn(string userName, string password);

        void CheckExistsBySubject(params string[] subjects);

        void CheckExistsByUserName(params string[] userNames);

        LoggedInUserModel SignIn(int clientId, string username, string password, out string refreshToken);

        Task<LoggedInUserModel> GetLoggedInUserBySubjectAsync(string subject);

        Task<LoggedInUserModel> GetLoggedInUserByRefreshTokenAsync(string refreshToken);

        /// <summary>
        ///     Expire all refresh token generated by sign in action, this method make user must sign
        ///     in again
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        Task ExpireAllRefreshTokenAsync(string subject);

        /// <summary>
        ///     Check refresh token is exist and not expire 
        /// </summary>
        /// <param name="clientId">    </param>
        /// <param name="refreshToken"></param>
        void CheckValidRefreshToken(int clientId, string refreshToken);

        /// <summary>
        ///     Active user via email, setup new username and password 
        /// </summary>
        /// <param name="subject">    </param>
        /// <param name="newUserName"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task ActiveByEmailAsync(string subject, string newUserName, string newPassword);

        Task ActiveByPhoneAsync(string subject, string newUserName, string newPassword);

        /// <summary>
        ///     Create User and Return Subject of new user 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<string> CreateUserByEmailAsync(string email);
    }
}