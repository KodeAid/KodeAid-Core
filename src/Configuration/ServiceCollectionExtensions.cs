// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection, string environment = null, string basePath = null, string jsonFileName = "appsettings.json")
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFileName, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (!string.IsNullOrEmpty(environment))
            {
                var fileName = Path.GetFileNameWithoutExtension(jsonFileName);
                var extension = Path.GetExtension(jsonFileName);
                builder.AddJsonFile($"{fileName}.{environment}{extension}", optional: true, reloadOnChange: true);
            }

            return serviceCollection.AddSingleton<IConfiguration>(builder.Build());
        }
    }
}
