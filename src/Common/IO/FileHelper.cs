// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Reflection;

namespace KodeAid.IO
{
    public static class FileHelper
    {
        public static string FindFile(string fileName)
        {
            ArgCheck.NotNullOrEmpty(nameof(fileName), fileName);
            if (TryFindFile(fileName, out var foundFileName))
            {
                return foundFileName;
            }

            throw new FileNotFoundException($"File '{fileName}' was not found.", fileName);
        }

        public static bool TryFindFile(string fileName, out string foundFileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                foundFileName = null;
                return false;
            }

            try
            {
                if (File.Exists(fileName))
                {
                    foundFileName = fileName;
                    return true;
                }

                if (!Path.IsPathRooted(fileName))
                {
#if NET8_0_OR_GREATER
                    var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#else
                    var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
#endif
                    var fn = exePath;
                    if (fn.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        fn = fn.Remove(0, "file://".Length);
                    }

                    if (fn.StartsWith("file:\\", StringComparison.OrdinalIgnoreCase))
                    {
                        fn = fn.Remove(0, "file:\\".Length);
                    }

                    fn = Path.GetFullPath(Path.Combine(fn, fileName));
                    if (File.Exists(fn))
                    {
                        foundFileName = fn;
                        return true;
                    }

#if NET8_0_OR_GREATER
                    var entryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#else
                    var entryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase);
#endif
                    if (exePath != entryPath)
                    {
                        fn = entryPath;
                        if (fn.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                        {
                            fn = fn.Remove(0, "file://".Length);
                        }

                        if (fn.StartsWith("file:\\", StringComparison.OrdinalIgnoreCase))
                        {
                            fn = fn.Remove(0, "file:\\".Length);
                        }

                        fn = Path.GetFullPath(Path.Combine(fn, fileName));
                        if (File.Exists(fn))
                        {
                            foundFileName = fn;
                            return true;
                        }
                    }
                }
            }
            catch { }

            foundFileName = null;
            return true;
        }
    }
}
