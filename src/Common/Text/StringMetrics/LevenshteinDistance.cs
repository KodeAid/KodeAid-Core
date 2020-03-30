// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Text.StringMetrics
{
    /// <summary>
    /// Compute the distance between two strings based on the number of
    /// individual character insertions, deletions or substitutions required to
    /// change one string into another.
    /// </summary>
    /// <remarks>
    /// The difference between Levenshtein and Damerau-Levenshtein is the exclusion
    /// of transpositions ('ae' => 'ea') in Levenshtein which count as two edits as
    /// opposed to just one in Damerau-Levenshtein.
    /// </remarks>
    public class LevenshteinDistance : IStringDistanceMetric
    {
        /// <summary>
        /// Compute the distance between two strings based on the number of
        /// individual character insertions, deletions or substitutions required to
        /// change <paramref name="strA"/> into <paramref name="strB"/>.
        /// </summary>
        public int ComputeDistance(string strA, string strB)
        {
            ArgCheck.NotNull(nameof(strA), strA);
            ArgCheck.NotNull(nameof(strB), strB);

            if (strA == strB)
            {
                return 0;
            }

            var strALength = strA.Length;
            var strBLength = strB.Length;

            // if strA has a length of 0 then the steps required to get to strB is the
            // length of strB as we simply would need to add the entire length of strB to strA
            if (strALength == 0)
            {
                return strBLength;
            }

            // if strB has a length of 0 then the steps required to get to strB is the
            // length of strA as we simply would need to remove the entire length of strA
            if (strBLength == 0)
            {
                return strALength;
            }

            // our distance matrix, strA is represneted by columns (x) and strB by rows (y)
            var distance = new int[strALength + 1, strBLength + 1];

            // preload left strA column (x)
            for (var a = 0; a <= strALength; distance[a, 0] = a++) ;

            // preload top strB row (y)
            for (var b = 0; b <= strBLength; distance[0, b] = b++) ;

            // compute the remaining matrix cells
            for (var a = 1; a <= strALength; a++)
            {
                for (var b = 1; b <= strBLength; b++)
                {
                    // requires an edit?
                    var cost = (strB[b - 1] == strA[a - 1]) ? 0 : 1;

                    // calculate distance for current cell
                    distance[a, b] = Math.Min(Math.Min(distance[a - 1, b] + 1, distance[a, b - 1] + 1), distance[a - 1, b - 1] + cost);
                }
            }

            // return distance
            return distance[strALength, strBLength];
        }
    }
}
