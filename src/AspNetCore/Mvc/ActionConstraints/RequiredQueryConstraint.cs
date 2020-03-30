// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace KodeAid.AspNetCore.Mvc.ActionConstraints
{
    public class RequiredQueryConstraint : IActionConstraint
    {
        private readonly string _name;

        public RequiredQueryConstraint(string name)
        {
            _name = name;
        }

        public int Order { get; set; } = 1000;

        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.Query.ContainsKey(_name);
        }
    }
}
