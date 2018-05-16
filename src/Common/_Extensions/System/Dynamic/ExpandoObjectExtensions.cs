// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using KodeAid;

namespace System.Dynamic
{
    public static class ExpandoObjectExtensions
    {
        public static ExpandoObject ExpandProperty(this ExpandoObject obj, string name)
        {
            ArgCheck.NotNull(nameof(obj), obj);
            ArgCheck.NotNullOrEmpty(nameof(name), name);
            var dictionary = (IDictionary<string, object>)obj;
            if (dictionary.TryGetValue(name, out object value) && value != null)
                obj = (ExpandoObject)value;
            else
                dictionary[name] = obj = new ExpandoObject();
            return obj;
        }
    }
}
