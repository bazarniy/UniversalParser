using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlStorage
{
    using System.IO;
    using Base;
    using Base.Utilities;

    public class DiskDriver : IStorageDriver
    {
        private readonly string _basePath;

        public DiskDriver(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            _basePath = path;

            if (path != string.Empty)
            {
                PathValidator.ValidateFolderPath(path);
                Directory.CreateDirectory(_basePath);
            }
        }

        public Stream FileWrite(string path)
        {
            return File.Create(GetValidatedPath(path));
        }

        public string GetRandomFileName()
        {
            return Path.GetRandomFileName();
        }

        public bool FileExist(string name)
        {
            return File.Exists(GetValidatedPath(name));
        }

        public Stream FileRead(string name)
        {
            var path = GetValidatedName(name);
            return !File.Exists(path) ? Stream.Null : File.OpenRead(path);
        }

        public IEnumerable<string> FileEnum()
        {
            return Directory.EnumerateFiles(_basePath);
        }

        public void FileRemove(string name)
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
