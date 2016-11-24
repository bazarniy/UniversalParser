namespace Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class PathValidator
    {
        public static void ValidateFilePath(string path)
        {
            BasePathValidation(path);

            var filename = GetLastPathSegment(path);
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("filename is empty");
            if (!IsValidFileNameChars(filename)) throw new ArgumentException($"invalid character in filename {filename}");

            FolderPathValidation(path, filename);
        }

        public static void ValidateFolderPath(string path)
        {
            BasePathValidation(path);
            FolderPathValidation(path);
        }

        public static IEnumerable<string> InvalidFolderCharacters()
        {
            var invalidPathChars = Path.GetInvalidPathChars().Select(x => x.ToString()).ToList();
            invalidPathChars.Add(":");
            invalidPathChars.Add(@"\\");
            invalidPathChars.Add(@"*");
            invalidPathChars.Add(@"?");
            return invalidPathChars;
        }

        private static bool IsShare(string path)
        {
            return path.StartsWith(@"\\");
        }

        private static void BasePathValidation(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("path is empty", nameof(path));
            if (IsShare(path)) throw new ArgumentException("is share path", nameof(path));
        }

        private static void FolderPathValidation(string path, string filename = "")
        {
            var label = GetPathRoot(path);
            if (!IsValidDiskLabel(label)) throw new ArgumentException($"wrong disk label '{label}'");

            var foldername = GetMiddlePathSegment(path, label, filename);
            if (InvalidFolderCharacters().Any(foldername.Contains)) throw new ArgumentException($"invalid character in folder '{foldername}'");
        }

        private static bool IsValidDiskLabel(string label)
        {
            return string.IsNullOrWhiteSpace(label)
                   || Environment.GetLogicalDrives().Contains(label.ToUpper(CultureInfo.CurrentCulture));
        }

        private static bool IsValidFileNameChars(string fileName)
        {
            return !Path.GetInvalidFileNameChars()
                .Select(x => x.ToString())
                .Any(fileName.Contains);
        }

        private static string GetLastPathSegment(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? string.Empty
                : path.Split(new[] {Path.DirectorySeparatorChar}, StringSplitOptions.None).LastOrDefault();
        }

        private static string GetMiddlePathSegment(string path, string label, string filename)
        {
            return path.Substring(0, path.Length - filename.Length).Substring(label.Length);
        }

        private static string GetPathRoot(string path)
        {
            var r = new Regex(@"^(([a-zA-Z]\:)|(\\))\\");
            var x = r.Match(path);
            if (!x.Success) return string.Empty;

            var result = x.Value;
            if (result.EndsWith(":"))
                result += "\\";

            return result;
        }
    }
}