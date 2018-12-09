// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using KodeAid.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace KodeAid.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromQueryAttribute : Microsoft.AspNetCore.Mvc.FromQueryAttribute, IParameterModelConvention
    {
        public FromQueryAttribute()
        {
        }

        public FromQueryAttribute(bool required)
        {
            Required = required;
        }

        public bool Required { get; set; }

        public void Apply(ParameterModel parameter)
        {
            if (Required && parameter.Action.Selectors != null && parameter.Action.Selectors.Count > 0)
            {
                parameter.Action.Selectors[parameter.Action.Selectors.Count - 1].ActionConstraints.Add(new RequiredQueryConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
            }
        }
    }
}
