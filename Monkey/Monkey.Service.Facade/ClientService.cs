﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey → Service Facade </Project>
//     <File>
//         <Name> ClientService.cs </Name>
//         <Created> 02/10/17 3:53:10 PM </Created>
//         <Key> d38f2f43-5176-4f70-8650-8ba418b45ff1 </Key>
//     </File>
//     <Summary>
//         ClientService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Monkey.Business;
using Monkey.Core.Models.Auth;
using Puppy.DependencyInjection.Attributes;
using System.Threading.Tasks;

namespace Monkey.Service.Facade
{
    [PerRequestDependency(ServiceType = typeof(IClientService))]
    public class ClientService : IClientService
    {
        private readonly IClientBusiness _clientBusiness;

        public ClientService(IClientBusiness ClientBusiness)
        {
            _clientBusiness = ClientBusiness;
        }

        public Task<ClientCreatedModel> CreateAsync(ClientCreateModel model)
        {
            _clientBusiness.CheckExistByName(model.Name);

            throw new System.NotImplementedException();
        }

        public Task<string> GenerateSecretAsync(int id)
        {
            _clientBusiness.CheckExist(id);
            throw new System.NotImplementedException();
        }
    }
}