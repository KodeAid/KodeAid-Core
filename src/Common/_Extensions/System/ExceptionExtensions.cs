// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string GetCombinedMessage(this Exception exception, string separator = " ")
        {
            var messages = new List<string>();

            while (exception != null)
            {
                if (exception is AggregateException aggregateException)
                {
                    exception = aggregateException.InnerExceptions.FirstOrDefault() ?? aggregateException.InnerException ?? aggregateException.GetBaseException();
                }

                var message = exception.Message?.TrimToNull();

                if (message != null && !messages.Any(m => m.Contains(message)))
                {
                    var p = message.Last();

                    if (p != '.' && p != '!' && p != '?')
                    {
                        message += ".";
                    }

                    messages.Add(message);
                }

                exception = exception.InnerException;
            }

            return string.Join(separator, messages);
        }

        public static bool IsTimeout(this Exception exception)
        {
            if (exception is TimeoutException)
            {
                return true;
            }

            if (exception is AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                {
                    var ex = innerException;

                    while (ex != null)
                    {
                        if (ex is TimeoutException)
                        {
                            return true;
                        }

                        ex = ex.InnerException;
                    }
                }
            }
            return false;
        }

        public static bool IsCanceled(this Exception exception)
        {
            if (exception is OperationCanceledException)
            {
                return true;
            }

            if (exception is AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                {
                    var ex = innerException;

                    while (ex != null)
                    {
                        if (ex is OperationCanceledException)
                        {
                            return true;
                        }

                        ex = ex.InnerException;
                    }
                }
            }
            return false;
        }
    }
}
