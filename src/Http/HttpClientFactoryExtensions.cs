// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Net.Http;

namespace Microsoft.Extensions.Http
{
    public static class HttpClientFactoryExtensions
    {
        public static HttpClient CreateClient<TClient>(this IHttpClientFactory httpClientFactory)
        {
            return httpClientFactory.CreateClient(typeof(TClient).FullName);
        }
    }
}
