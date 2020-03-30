// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class MacroConstantCaseNamingStrategy : CamelCaseNamingStrategy
    {
        public MacroConstantCaseNamingStrategy()
            : base()
        {

        }

        public MacroConstantCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
            : base(processDictionaryKeys, overrideSpecifiedNames)
        {

        }

        public MacroConstantCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames, bool processExtensionDataNames)
            : base(processDictionaryKeys, overrideSpecifiedNames, processExtensionDataNames)
        {

        }

        protected override string ResolvePropertyName(string name) => base.ResolvePropertyName(name)?.ToMacroConstantCase();
    }
}
