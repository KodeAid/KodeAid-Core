// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.AspNetCore.Rewrite;

namespace Microsoft.AspNetCore.Rewrite
{
    public static class RewriteOptionsExtensions
    {
        public static RewriteOptions AddRedirectToWww(this RewriteOptions options)
        {
            return options.Add(new WwwRule());
        }
    }
}
