﻿#region	License

//------------------------------------------------------------------------------------------------
// <Auto-generated>
//     <Author> Top Nguyen (http://topnguyen.net) </Author>
//     <Project> Monkey.Api </Project>
//     <File> 
//         <Name> Program.cs </Name>
//         <Created> 28 03 2017 10:19:01 AM </Created>
//         <Key> 29832439-583C-47C6-A80F-B93105D7109D </Key>
//     </File>
//     <Summary>
//         Program
//     </Summary>
// </Auto-generated>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Monkey.Core;
using Monkey.Core.Configs;
using Monkey.Extensions;
using Puppy.Core.EnvironmentUtils;

namespace Monkey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.Title = $"Welcome [{EnvironmentHelper.MachineName}], [{PlatformServices.Default.Application.ApplicationName}] App (v{PlatformServices.Default.Application.ApplicationVersion}) - Env [{EnvironmentHelper.Name}] | {PlatformServices.Default.Application.RuntimeFramework.FullName} | {RuntimeInformation.OSDescription}";

            // Build System Config at first time for config Root, in Startup will build again with reload update features
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            IConfiguration configuration = builder.Build();

            SystemConfigurationHelper.BuildSystemConfig(configuration);

            IWebHostBuilder hostBuilder =
                new WebHostBuilder()
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                    })
                    .UseWebRoot(SystemConfig.MvcPath.WebRootFolderName)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();

            hostBuilder.Build().Run();
        }
    }
}