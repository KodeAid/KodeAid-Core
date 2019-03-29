// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace KodeAid.Azure.KeyVault
{
    public class ManagedServiceIdentityKeyVaultClient : KeyVaultClient
    {
        private static readonly AzureServiceTokenProvider _tokenProvider = new AzureServiceTokenProvider();

        public ManagedServiceIdentityKeyVaultClient()
            : base(new AuthenticationCallback(_tokenProvider.KeyVaultTokenCallback))
        {

        }
    }
}
