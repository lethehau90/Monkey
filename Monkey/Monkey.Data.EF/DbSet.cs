﻿#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> DbSet.cs </Name>
//         <Created> 18/04/2017 10:46:11 AM </Created>
//         <Key> 34786a69-00f1-46ef-b365-4414a7def4aa </Key>
//     </File>
//     <Summary>
//         DbSet.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore;
using Monkey.Core.Entities;
using Monkey.Core.Entities.Auth;
using Monkey.Core.Entities.User;

namespace Monkey.Data.EF
{
    public sealed partial class DbContext
    {
        #region Auth

        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        public DbSet<ClientEntity> Clients { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        public DbSet<PermissionEntity> Permissions { get; set; }

        #endregion

        #region User

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<ProfileEntity> Profiles { get; set; }

        #endregion

        public DbSet<ImageEntity> Images { get; set; }

        public DbSet<ConfigurationEntity> Configurations { get; set; }
    }
}