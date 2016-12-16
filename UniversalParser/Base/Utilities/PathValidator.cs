namespace Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Helpers;

    public static class PathValidator
    {
        public static void ValidateFilePath(string path)
        {
            BasePathValidation(path);

            var filename = GetLastPathSegment(path);
            filename.ThrowIfEmpty(nameof(filename), "filename is empty");
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

        public static string GetLastPathSegment(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? string.Empty
                : path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.None).LastOrDefault();
        }

        public static void ValidateExtention(string extention)
        {
            if (extention.IsEmpty()) return;
            BasePathValidation(extention);
            if (!IsValidFileNameChars(extention)) throw new ArgumentException($"invalid character in extention {extention}");
            if (InvalidFolderCharacters().Any(extention.Contains)) throw new ArgumentException($"invalid character in extention '{extention}'");
            if (extention.Contains(".")) throw new ArgumentException($"invalid character in extention '{extention}'");
        }

        private static bool IsShare(string path)
        {
            return path.StartsWith(@"\\");
        }
        private static bool IsMultiple(string path)
        {
            return path.Contains(Path.PathSeparator);
        }

        private static void BasePathValidation(string path)
        {
            path.ThrowIfNull(nameof(path));
            path.ThrowIfEmpty(nameof(path));
            if (IsShare(path)) throw new ArgumentException("is share path", nameof(path));
            if (IsMultiple(path)) throw new ArgumentException("is multiple path", nameof(path));
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
            return label.IsEmpty() || Environment.GetLogicalDrives().Contains(label.ToUpper(CultureInfo.CurrentCulture));
        }

        private static bool IsValidFileNameChars(string fileName)
        {
            return !Path.GetInvalidFileNameChars()
                .Select(x => x.ToString())
                .Any(fileName.Contains);
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