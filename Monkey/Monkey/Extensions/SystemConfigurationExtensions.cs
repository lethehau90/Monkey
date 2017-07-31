﻿#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> SystemConfigurationExtensions.cs </Name>
//         <Created> 31/07/17 10:43:11 PM </Created>
//         <Key> 8d82c0f2-ef77-4bdb-9ff2-8e00e66dc71f </Key>
//     </File>
//     <Summary>
//         SystemConfigurationExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Monkey.Core.ConfigModels;
using Puppy.Core.ConfigUtils;
using Puppy.Core.EnvironmentUtils;
using Puppy.DependencyInjection;

namespace Monkey.Extensions
{
    public static class SystemConfigurationExtensions
    {
        /// <summary>
        ///     I will add config into 2 places: <br />
        ///     - First place is in SystemConfig object: this will content all config. <br />
        ///     - Second place is in separate object: this for lightweight inject and read data.
        /// </summary>
        /// <param name="services">          </param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="configurationRoot"> </param>
        public static IServiceCollection AddSystemConfigurationMonkey(this IServiceCollection services, IHostingEnvironment hostingEnvironment, IConfigurationRoot configurationRoot)
        {
            // Add Service
            services.AddSingleton(hostingEnvironment);
            services.AddSingleton(configurationRoot);
            services.AddSingleton<IConfiguration>(configurationRoot);

            // Build System Config
            SystemConfigurationHelper.BuildSystemConfig(configurationRoot);

            return services;
        }

        public static IApplicationBuilder UseSystemConfigurationMonkey(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Currently, ASPNETCORE have an issue hit twice when change appsetting.json from
            // 20/03/17 (see more: https://github.com/aspnet/SystemConfigs/issues/624) And please
            // don't use IOption and IOptionSnapshot, it harder to manage lifetime of object and mad
            // of Injection I assumption configuration is singleton and use Statics class to do it.
            // Keep Simple everything Possible.

            IConfigurationRoot configurationRoot = app.ApplicationServices.Resolve<IConfigurationRoot>();

            ChangeToken.OnChange(configurationRoot.GetReloadToken, () =>
            {
                // Build System Config
                SystemConfigurationHelper.BuildSystemConfig(configurationRoot);

                loggerFactory.CreateLogger<Startup>().LogWarning("System Configuration Changed!");
            });

            return app;
        }
    }

    public static class SystemConfigurationHelper
    {
        public static void BuildSystemConfig(IConfiguration configuration)
        {
            // Database Connection String
            GetDatabaseConnectionStringConfig(configuration);

            GetMvcPathConfig(configuration);

            Core.SystemConfigs.Serilog = configuration.GetSection<SerilogConfigModel>(nameof(Core.SystemConfigs.Serilog));

            Core.SystemConfigs.Developers = configuration.GetSection<DevelopersConfigModel>(nameof(Core.SystemConfigs.Developers));

            Core.SystemConfigs.Server = configuration.GetSection<ServerConfigModel>(nameof(Core.SystemConfigs.Server));

            Core.SystemConfigs.IdentityServer = configuration.GetSection<IdentityServerConfigModel>(nameof(Core.SystemConfigs.IdentityServer));

            Core.SystemConfigs.DistributedCache = configuration.GetSection<DistributedCacheConfigModel>(nameof(Core.SystemConfigs.DistributedCache));

            Core.SystemConfigs.Elastic = configuration.GetSection<ElasticConfigModel>(nameof(Core.SystemConfigs.Elastic));
        }

        private static void GetDatabaseConnectionStringConfig(IConfiguration configuration)
        {
            string databaseConnectionString =
                configuration
                    .GetValue<string>(
                        (EnvironmentHelper.IsProduction() || EnvironmentHelper.IsStaging())
                            ? $"ConnectionStrings:{EnvironmentHelper.Name}"
                            : $"ConnectionStrings:{EnvironmentHelper.MachineName}");

            // Database Connection String is Simple Type so it still exist in SystemConfig object
            Core.SystemConfigs.DatabaseConnectionString = databaseConnectionString;
        }

        private static void GetMvcPathConfig(IConfiguration configuration)
        {
            Core.SystemConfigs.MvcPath = configuration.GetSection<MvcPathConfigModel>(nameof(Core.SystemConfigs.MvcPath));
        }
    }
}