// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Text.Normalization
{
    /// <summary>
    /// Provides an abstraction for normalizing keys for lookup purposes.
    /// </summary>
    public interface ILookupNormalizer
    {
        /// <summary>
        /// Returns a normalized representation of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to normalize.</param>
        /// <returns>A normalized representation of the specified <paramref name="key"/>.</returns>
        string Normalize(string key);
    }
}
