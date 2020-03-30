// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Globalization;

namespace Microsoft.Extensions.WebEncoders.Sources
{
    /// <summary>
    /// https://github.com/aspnet/Extensions/blob/5c3ee37fb773db04a1aea6b67e49395e84fda223/shared/Microsoft.Extensions.WebEncoders.Sources/Properties/EncoderResources.cs
    /// </summary>
    internal static class EncoderResources
    {
        /// <summary>
        /// Invalid {0}, {1} or {2} length.
        /// </summary>
        internal static readonly string WebEncoders_InvalidCountOffsetOrLength = "Invalid {0}, {1} or {2} length.";

        /// <summary>
        /// Malformed input: {0} is an invalid input length.
        /// </summary>
        internal static readonly string WebEncoders_MalformedInput = "Malformed input: {0} is an invalid input length.";

        /// <summary>
        /// Invalid {0}, {1} or {2} length.
        /// </summary>
        internal static string FormatWebEncoders_InvalidCountOffsetOrLength(object p0, object p1, object p2) => string.Format(CultureInfo.CurrentCulture, WebEncoders_InvalidCountOffsetOrLength, p0, p1, p2);

        /// <summary>
        /// Malformed input: {0} is an invalid input length.
        /// </summary>
        internal static string FormatWebEncoders_MalformedInput(object p0) => string.Format(CultureInfo.CurrentCulture, WebEncoders_MalformedInput, p0);
    }
}
