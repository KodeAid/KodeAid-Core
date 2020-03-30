// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Text.StringMetrics
{
    /// <summary>
    /// Provides an abstraction for calculating the distance between two strings.
    /// </summary>
    public interface IStringDistanceMetric
    {
        /// <summary>
        /// Compute the distance between two strings based on the number of
        /// individual character edits required to change <paramref name="strA"/> into <paramref name="strB"/>.
        /// </summary>
        int ComputeDistance(string strA, string strB);
    }
}
