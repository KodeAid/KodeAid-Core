// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Net.Http;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace KodeAid.Azure.KeyVault
{
    public class ManagedServiceIdentityKeyVaultClient : KeyVaultClient
    {
        private static readonly AzureServiceTokenProvider _tokenProvider = new AzureServiceTokenProvider();

        // Due to an issue with the internal HttpClient being left open or not disposed of properly,
        // this shuold help.
        // https://github.com/Azure/azure-sdk-for-net/issues/6003
        // Although the only issue now is that DNS changes may not be noticed on the key vault hosts.
        // https://softwareengineering.stackexchange.com/questions/330364/should-we-create-a-new-single-instance-of-httpclient-for-all-requests
        private static readonly HttpClient _httpClient = new HttpClient();

        public ManagedServiceIdentityKeyVaultClient()
            : base(new AuthenticationCallback(_tokenProvider.KeyVaultTokenCallback), _httpClient)
        {
        }
    }
}
