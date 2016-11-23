using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Utilities
{
    using System.Text.RegularExpressions;

    public static class PathValidator
    {
        public static void ValidateFilePath(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("path is empty", nameof(path));
            if (IsShare(path)) throw new ArgumentException("is share path", nameof(path));

            var label = GetPathRoot(path);
            if (!ValidateDiskLabel(label)) throw new ArgumentException($"wrong disk label '{label}'");
        }


        private static bool IsShare(string path)
        {
            return path.StartsWith(@"\\");
        }

        private static bool ValidateDiskLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return true;
            return label == @"C:\";
        }

        private static string GetPathRoot(string path)
        {
            var r = new Regex(@"^(([a-zA-Z]\:)|(\\))\\");
            var x = r.Match(path);
            if (!x.Success) return null;

            var result = x.Value;
            if (result.EndsWith(":"))
            {
                result += "\\";
            }

            return result;
        }
    }
}
