// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using KodeAid.FaultTolerance;
using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace KodeAid.Net.Http
{
    public class HttpOperationManager : OperationManager<int>
    {
        public HttpOperationManager(IRetryPolicy retryPolicy = null)
            : base(retryPolicy)
        {
        }

        protected override bool CheckIsSuccess(object state, int statusCode, Exception exception)
        {
            if (statusCode >= 100 && statusCode < 400 && exception == null)
            {
                return true;
            }

            return false;
        }

        protected override bool CheckIsRetryable(object state, int statusCode, Exception exception)
        {
            if (statusCode >= 500 && statusCode < 600)
            {
                return true;
            }

            // look for a SocketException (root cause) and see if its retryable
            var ex = exception;
            while (ex != null)
            {
                if (ex is SocketException socketException &&
                    CheckSocketExceptionIsRetryable(socketException))
                {
                    return true;
                }
                ex = ex.InnerException;
            }

            // look for an HttpRequestException (high-level) and see if its retryable
            ex = exception;
            while (ex != null)
            {
                if (ex is HttpRequestException httpRequestException &&
                    CheckHttpRequestExceptionIsRetryable(httpRequestException))
                {
                    return true;
                }
                ex = ex.InnerException;
            }

            return false;
        }

        public static bool CheckHttpRequestExceptionIsRetryable(HttpRequestException httpRequestException)
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

        public static bool CheckSocketExceptionIsRetryable(SocketException socketException)
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
