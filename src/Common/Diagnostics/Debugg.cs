// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KodeAid.Diagnostics
{
    public static class Debugg
    {
        [Conditional("DEBUG")]
        public static void WriteLine(object source, string message = null, Exception ex = null, [CallerMemberName] string methodName = null)
        {
            WriteLine(source.GetType(), methodName, message, ex);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(object source, Exception ex, [CallerMemberName] string methodName = null)
        {
            WriteLine(source.GetType(), methodName, null, ex);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(Type type, string message = null, Exception ex = null, [CallerMemberName] string methodName = null)
        {
            WriteLine(type, methodName, message, ex);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(Type type, Exception ex, [CallerMemberName] string methodName = null)
        {
            WriteLine(type, methodName, null, ex);
        }

        private static void WriteLine(Type type, string methodName, string message, Exception ex)
        {
            var typeName = string.Empty;
            methodName = methodName ?? string.Empty;
            message = message ?? string.Empty;
            var exMessage = ex?.ToString() ?? string.Empty;
            if (type != null)
            {
                typeName = type.Name;
                var typeInfo = type.GetTypeInfo();
                if (typeInfo != null && typeInfo.IsGenericType)
                {
                    if (typeName.Contains("`"))
                        typeName = typeName.Substring(0, typeName.IndexOf('`'));
                    typeName += '<';
                    typeName += string.Join(",", typeInfo.GenericTypeArguments.Select(a => a.Name));
                    typeName += '>';
                }
            }
            if (!string.IsNullOrEmpty(typeName))
                typeName = " " + typeName;
            if (!string.IsNullOrEmpty(methodName))
                methodName = " " + methodName;
            if (!string.IsNullOrEmpty(message))
                message = " " + message;
            if (!string.IsNullOrEmpty(exMessage))
                exMessage = " " + exMessage;
            Debug.WriteLine($"{typeName}{methodName}{message}{exMessage}");
        }
    }
}
