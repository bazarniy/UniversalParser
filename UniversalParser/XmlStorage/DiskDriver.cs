namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Base.Utilities;

    public class DiskDriver : IStorageDriver
    {
        private readonly string _basePath;

        public DiskDriver(string path)
        {
            path.ThrowIfNull(nameof(path));

            _basePath = path;

            if (path == string.Empty) return;

            PathValidator.ValidateFolderPath(path);
            Directory.CreateDirectory(_basePath);
        }

        public Stream Write(string name)
        {
            return File.Create(GetValidatedPath(name));
        }

        public string GetRandomName()
        {
            return Path.GetRandomFileName();
        }

        public bool Exists(string name)
        {
            return File.Exists(GetValidatedPath(name));
        }

        public Stream Read(string name)
        {
            var path = GetValidatedPath(name);
            return !File.Exists(path) ? Stream.Null : File.OpenRead(path);
        }

        public IEnumerable<string> Enum()
        {
            return Directory.EnumerateFiles(_basePath).Select(x=>x.Substring(_basePath.Length+1));
        }

        public void Remove(string name)
        {
            File.Delete(GetValidatedPath(name));
        }

        public long GetLength(string name)
        {
            var path = GetValidatedPath(name);
            return !File.Exists(path) ? 0 : new FileInfo(path).Length;
        }

        private string GetValidatedPath(string path)
        {
            return GetPath(GetValidatedName(path));
        }

        private string GetPath(string name)
        {
            return Path.Combine(_basePath, name);
        }

        private static string GetValidatedName(string name)
        {
            name.ThrowIfNull(nameof(name));
            name.ThrowIfEmpty(nameof(name));

            var fileName = PathValidator.GetLastPathSegment(name);
            fileName.ThrowIfEmpty(nameof(name));

            PathValidator.ValidateFilePath(fileName);
            return fileName;
        }
    }
}
