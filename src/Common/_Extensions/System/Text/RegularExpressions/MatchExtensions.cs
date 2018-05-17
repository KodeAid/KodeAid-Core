// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Linq;
using KodeAid;

namespace System.Text.RegularExpressions
{
    public static class MatchExtensions
    {
        public static string GetFirstInnerMostGroupCapture(this Match match)
        {
            ArgCheck.NotNull(nameof(match), match);
            if (match.Success && match.Groups != null)
            {
                var matchGroup = match.Groups.Cast<Group>().LastOrDefault();
                if (matchGroup != null && matchGroup.Success && matchGroup.Captures != null)
                {
                    var matchCapture = matchGroup.Captures.Cast<Capture>().LastOrDefault();
                    if (matchCapture != null)
                    {
                        return matchCapture.Value;
                    }
                }
            }
            return null;
        }

        public static string JoinAllCaptures(this MatchCollection matches, string separator = "")
        {
            ArgCheck.NotNull(nameof(matches), matches);
            return string.Join(separator, matches.OfType<Match>().Select(m => m.GetFirstInnerMostGroupCapture()));
        }
    }
}
