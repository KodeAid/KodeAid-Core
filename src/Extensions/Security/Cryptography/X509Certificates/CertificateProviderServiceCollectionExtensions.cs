// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;
using KodeAid.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CertificateProviderServiceCollectionExtensions
    {
        public static void AddCertificateProvider(this IServiceCollection services)
        {
            ArgCheck.NotNull(nameof(services), services);
            services.AddTransient<ICertificateProvider, CertificateProvider>();
        }
    }
}
