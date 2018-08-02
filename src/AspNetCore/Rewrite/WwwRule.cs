// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;

namespace KodeAid.AspNetCore.Rewrite
{
    public class WwwRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            var host = context?.HttpContext?.Request?.Host.Host;
            if (!string.IsNullOrEmpty(host) && !host.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
            {
                var url = context.HttpContext.Request.GetDisplayUrl();
                context.HttpContext.Response.Redirect(url.Insert(url.IndexOf("://") + 3, "www."), true);
                context.Result = RuleResult.EndResponse;
            }
        }
    }
}
