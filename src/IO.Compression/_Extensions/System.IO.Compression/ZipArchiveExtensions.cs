// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace System.IO.Compression
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (var entry in archive.Entries)
            {
                var completeFileName = Path.Combine(destinationDirectoryName, entry.FullName);
                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(completeFileName);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    entry.ExtractToFile(completeFileName, true);
                }
            }
        }
    }
}
