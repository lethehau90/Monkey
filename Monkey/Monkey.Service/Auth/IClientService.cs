﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey → Service Interface </Project>
//     <File>
//         <Name> IClientService.cs </Name>
//         <Created> 02/10/17 3:48:36 PM </Created>
//         <Key> 4b604e36-7c6f-489a-9242-c9e466d595ec </Key>
//     </File>
//     <Summary>
//         IClientService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Monkey.Core.Models.Auth;
using Puppy.DataTable.Models.Request;
using Puppy.DataTable.Models.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Monkey.Service.Auth
{
    public interface IClientService : IBaseService
    {
        #region Get

        Task<DataTableResponseDataModel<ClientModel>> GetDataTableAsync(DataTableParamModel model, CancellationToken cancellationToken = default);

        Task<ClientModel> GetAsync(int id, CancellationToken cancellationToken = default);

        #endregion

        #region Create

        Task<int> CreateAsync(CreateClientModel model, CancellationToken cancellationToken = default);

        #endregion

        #region Update

        Task UpdateAsync(UpdateClientModel model, CancellationToken cancellationToken = default);

        Task<string> GenerateSecretAsync(int id, CancellationToken cancellationToken = default);

        #endregion

        #region Remove

        Task RemoveAsync(int id, CancellationToken cancellationToken = default);

        #endregion

        #region Validation

        void CheckUniqueName(string name, int? excludeId);

        #endregion
    }
}