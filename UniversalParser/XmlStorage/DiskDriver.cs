namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Base.Utilities;

    public class DiskDriver : IStorageDriver
    {
        private readonly string _basePath;

        public DiskDriver(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
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
            var path = GetValidatedName(name);
            return !File.Exists(path) ? Stream.Null : File.OpenRead(path);
        }

        public IEnumerable<string> Enum()
        {
            return Directory.EnumerateFiles(_basePath);
        }

        public void Remove(string name)
        {
            File.Delete(GetValidatedPath(name));
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
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("file name is empty", nameof(name));

            var fileName = PathValidator.GetLastPathSegment(name);
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("file name is empty", nameof(name));

            PathValidator.ValidateFilePath(fileName);
            return fileName;
        }
    }
}
