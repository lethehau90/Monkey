﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey → Business Logic </Project>
//     <File>
//         <Name> RoleBusiness.cs </Name>
//         <Created> 06/10/17 10:56:22 PM </Created>
//         <Key> 6b87476d-0015-437b-8c41-dda12a8dce0f </Key>
//     </File>
//     <Summary>
//         RoleBusiness.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Monkey.Business.Auth;
using Monkey.Core.Entities.Auth;
using Monkey.Core.Exceptions;
using Monkey.Core.Models;
using Monkey.Core.Models.Auth;
using Monkey.Data.Auth;
using Puppy.AutoMapper;
using Puppy.Core.StringUtils;
using Puppy.DependencyInjection.Attributes;
using Puppy.Web.Models.Api;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enums = Monkey.Core.Constants.Enums;

namespace Monkey.Business.Logic.Auth
{
    [PerRequestDependency(ServiceType = typeof(IRoleBusiness))]
    public class RoleBusiness : IRoleBusiness
    {
        private readonly IRoleRepository _roleRepository;

        public RoleBusiness(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        #region Get

        public Task<RoleModel> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var roleModel = _roleRepository.Get(x => x.Id == id).QueryTo<RoleModel>().FirstOrDefault();
            return Task.FromResult(roleModel);
        }

        public Task<PagedCollectionResultModel<RoleModel>> GetListRoleAsync(PagedCollectionParametersModel model, CancellationToken cancellationToken = default)
        {
            var query = _roleRepository.Get();

            if (!string.IsNullOrWhiteSpace(model.Terms))
            {
                var termsNorm = StringHelper.Normalize(model.Terms);

                query = query.Where(x => x.NameNorm.Contains(termsNorm));
            }

            var total = query.LongCount();

            // Check cancellation token
            cancellationToken.ThrowIfCancellationRequested();

            var listRoleModel = query.OrderByDescending(x => x.NameNorm).Skip(model.Skip).Take(model.Take).QueryTo<RoleModel>().ToList();

            var result = new PagedCollectionResultModel<RoleModel>
            {
                Terms = model.Terms,
                Take = model.Take,
                Skip = model.Skip,
                Items = listRoleModel,
                Total = total
            };

            return Task.FromResult(result);
        }

        public Task<int> GetMemberRoleIdAsync()
        {
            int id = _roleRepository.Get(x => x.Permissions.Any(y => y.Permission == Enums.Permission.Member)).Select(x => x.Id).FirstOrDefault();

            return Task.FromResult(id);
        }

        #endregion

        #region Create

        public Task<int> CreateAsync(string name, string description, CancellationToken cancellationToken = default, params Enums.Permission[] permissions)
        {
            var roleEntity = new RoleEntity
            {
                Name = name,
                NameNorm = StringHelper.Normalize(name),
                Description = description,
                Permissions = permissions?.Select(x => new PermissionEntity
                {
                    Permission = x
                }).ToList()
            };

            _roleRepository.Add(roleEntity);

            // Check cancellation token
            cancellationToken.ThrowIfCancellationRequested();

            _roleRepository.SaveChanges();

            return Task.FromResult(roleEntity.Id);
        }

        #endregion

        #region Validation

        public void CheckUniqueName(string name, int? excludeId = null)
        {
            var nameNorm = StringHelper.Normalize(name);

            var isExist = _roleRepository.Get(x => x.NameNorm == nameNorm).Any();

            if (isExist)
            {
                throw new MonkeyException(ErrorCode.UserNameNotUnique);
            }
        }

        #endregion
    }
}