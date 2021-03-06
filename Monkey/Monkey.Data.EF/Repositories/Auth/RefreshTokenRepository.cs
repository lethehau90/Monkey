﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey → Repository </Project>
//     <File>
//         <Name> RefreshTokenRepository.cs </Name>
//         <Created> 13/09/17 11:25:19 PM </Created>
//         <Key> b8e0132e-39d5-481b-a6c1-0a4534af34f4 </Key>
//     </File>
//     <Summary>
//         RefreshTokenRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Monkey.Core.Entities.Auth;
using Monkey.Data.Auth;
using Puppy.DependencyInjection.Attributes;

namespace Monkey.Data.EF.Repositories.Auth
{
    [PerRequestDependency(ServiceType = typeof(IRefreshTokenRepository))]
    public class RefreshTokenRepository : EntityRepository<RefreshTokenEntity>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}