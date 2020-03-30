// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Text.StringMetrics
{
    public static class StringExtensions
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
        public static int ComputeLevenshteinDistance(this string source, string target)
        {
            return new LevenshteinDistance().ComputeDistance(source, target);
        }

        /// <summary>
        /// Compute the distance between two strings based on the number of
        /// individual character insertions, deletions, substitutions or transpositions
        /// required to change one string into another.
        /// </summary>
        /// <remarks>
        /// The difference between Damerau-Levenshtein and Levenshtein is the inclusion
        /// of transpositions ('ae' => 'ea') in Damerau-Levenshtein which count as a
        /// single edit as opposed to two in Levenshtein.
        /// </remarks>
        public static int ComputeDamerauLevenshteinDistance(this string source, string target)
        {
            return new DamerauLevenshteinDistance().ComputeDistance(source, target);
        }
    }
}
