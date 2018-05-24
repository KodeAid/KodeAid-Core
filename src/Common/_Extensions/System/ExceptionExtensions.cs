// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string GetCombinedMessage(this Exception ex, string separator = " ")
        {
            var m = new List<string>();
            while (ex != null)
            {
                if (ex is AggregateException a)
                    ex = a.InnerExceptions.FirstOrDefault() ?? a.InnerException ?? a.GetBaseException();
                m.Add(ex.Message.EndsWith(".") ? ex.Message : (ex.Message + "."));
                ex = ex.InnerException;
            }
            return string.Join(separator, m);
        }

        public static bool IsTimeout(this Exception exception)
        {
            if (exception is TimeoutException)
                return true;
            if (exception is AggregateException)
            {
                foreach (var innerException in ((AggregateException)exception).Flatten().InnerExceptions)
                {
                    var ex = innerException;
                    while (ex != null)
                    {
                        if (ex is TimeoutException)
                            return true;
                        ex = ex.InnerException;
                    }
                }
            }
            return false;
        }

        public static bool IsCanceled(this Exception exception)
        {
            if (exception is OperationCanceledException)
                return true;
            if (exception is AggregateException)
            {
                foreach (var innerException in ((AggregateException)exception).Flatten().InnerExceptions)
                {
                    var ex = innerException;
                    while (ex != null)
                    {
                        if (ex is OperationCanceledException)
                            return true;
                        ex = ex.InnerException;
                    }
                }
            }
            return false;
        }
    }
}
