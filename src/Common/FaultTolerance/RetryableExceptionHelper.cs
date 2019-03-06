// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace KodeAid.FaultTolerance
{
    public static class RetryableExceptionHelper
    {
        public static bool CheckForRetryableException(Exception exception)
        {
            return CheckForRetryableException<IRetryable>(exception, retryable => retryable.CanRetry);
        }

        public static bool CheckForRetryableException<T>(Exception exception, Func<T, bool> canRetry = null, bool skipAggregateExceptions = false)
        {
            ArgCheck.NotNull(nameof(canRetry), canRetry);

            if (exception == null)
            {
                return false;
            }

            var exceptions = new Queue<Exception>();
            exceptions.Enqueue(exception);

            while (exceptions.Count > 0)
            {
                exception = exceptions.Dequeue();
                if (exception == null)
                {
                    continue;
                }

                if (exception is T ex)
                {
                    if (canRetry == null)
                    {
                        return true;
                    }

                    if (canRetry(ex))
                    {
                        return true;
                    }
                }

                if (!skipAggregateExceptions && exception is AggregateException aggregateException)
                {
                    exceptions.EnqueueRange(aggregateException.InnerExceptions.WhereNotNull());
                }

                if (exception.InnerException != null)
                {
                    exceptions.Enqueue(exception.InnerException);
                }
            }

            return false;
        }

        public static bool CheckForRetryableHttpRequestException(Exception exception)
        {
            return CheckForRetryableException<HttpRequestException>(exception, IsHttpRequestExceptionRetryable);
        }

        public static bool CheckForRetryableSocketException(Exception exception)
        {
            return CheckForRetryableException<SocketException>(exception, IsSocketExceptionRetryable);
        }

        public static bool CheckForRetryableTimeoutException(Exception exception)
        {
            return CheckForRetryableException<TimeoutException>(exception);
        }

        public static bool IsHttpRequestExceptionRetryable(HttpRequestException httpRequestException)
        {
            ArgCheck.NotNull(nameof(httpRequestException), httpRequestException);

            // very ugly, but let's try to parse a 3-digit status code from the message.
            var matches = Regex.Matches(httpRequestException.Message, @"(?:^|\D)(\d\d\d)(?:$|\D)", RegexOptions.Compiled);
            if (matches.Count == 1)
            {
                var statusCodeAsString = matches[0].GetFirstInnerMostGroupCapture();
                if (int.TryParse(statusCodeAsString, out var statusCode)
                    && statusCode >= 500 && statusCode < 600)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSocketExceptionRetryable(SocketException socketException)
        {
            ArgCheck.NotNull(nameof(socketException), socketException);

            switch (socketException.SocketErrorCode)
            {
                case SocketError.AccessDenied:
                case SocketError.ConnectionAborted:
                case SocketError.ConnectionRefused:
                case SocketError.ConnectionReset:
                case SocketError.Disconnecting:
                case SocketError.Fault:
                case SocketError.HostDown:
                case SocketError.HostNotFound:
                case SocketError.HostUnreachable:
                case SocketError.InProgress:
                case SocketError.Interrupted:
                case SocketError.IOPending:
                case SocketError.IsConnected:
                case SocketError.MessageSize:
                case SocketError.NetworkDown:
                case SocketError.NetworkReset:
                case SocketError.NetworkUnreachable:
                case SocketError.NoBufferSpaceAvailable:
                case SocketError.NoData:
                case SocketError.NoRecovery:
                case SocketError.NotConnected:
                case SocketError.NotInitialized:
                case SocketError.NotSocket:
                case SocketError.OperationAborted:
                case SocketError.ProcessLimit:
                case SocketError.ProtocolOption:
                case SocketError.Shutdown:
                case SocketError.SocketError:
                case SocketError.SystemNotReady:
                case SocketError.TimedOut:
                case SocketError.TooManyOpenSockets:
                case SocketError.TryAgain:
                case SocketError.TypeNotFound:
                case SocketError.WouldBlock:
                    return true;

                case SocketError.AddressAlreadyInUse:
                case SocketError.AddressFamilyNotSupported:
                case SocketError.AddressNotAvailable:
                case SocketError.AlreadyInProgress:
                case SocketError.DestinationAddressRequired:
                case SocketError.InvalidArgument:
                case SocketError.OperationNotSupported:
                case SocketError.ProtocolFamilyNotSupported:
                case SocketError.ProtocolNotSupported:
                case SocketError.ProtocolType:
                case SocketError.SocketNotSupported:
                case SocketError.Success:
                case SocketError.VersionNotSupported:
                default:
                    break;
            }

            return false;
        }
    }
}
