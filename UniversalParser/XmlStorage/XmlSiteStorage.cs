namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Base;
    using Base.Serializers;

    public class XmlSiteStorage : ISiteStorage
    {
        private const string IndexName = "index.xml";
        private readonly string _basePath;
        private readonly XmlStorageIndex _index;
        private readonly string _indexPath;

        private readonly object _latch = new object(); //TODO: to reader writer lock

        public XmlSiteStorage(string path)
        {
            _basePath = Path.GetDirectoryName(path) ?? "";
            if (!string.IsNullOrWhiteSpace(_basePath)) Directory.CreateDirectory(_basePath);
            _indexPath = GetStorageFilePath(IndexName);

            _index = File.Exists(_indexPath) ? XmlClassSerializer.Load<XmlStorageIndex>(_indexPath) : new XmlStorageIndex();

        }

        public IEnumerable<string> GetFileNames()
        {
            lock (_latch)
            {
                return _index.Items.Select(x => x.FileName).ToArray();
            }
        }

        public DataInfo GetFile(string path)
        {
            var filename = Path.GetFileName(path);
            lock (_latch)
            {
                if (_index.Items.All(x => x.FileName != filename))
                    throw new ApplicationException($"There is no file {filename}");
            }

            return GetFileInternal(filename);
        }

        public IEnumerable<DataInfo> GetInfos()
        {
            var files = GetFileNames();
            foreach (var file in files)
                yield return GetFileInternal(file);
        }

        public int Count()
        {
            lock (_latch)
            {
                return _index.Items.Count;
            }
        }

        public void Write(DataInfo info)
        {
            lock (_latch)
            {
                var filename = _index.Items.FirstOrDefault(x => x.Url == info.Url)?.FileName;
                if (string.IsNullOrWhiteSpace(filename))
                {
                    _index.Items.Add(new XmlStorageItem {FileName = Path.GetTempFileName(), Url = info.Url});
                    XmlClassSerializer.Save(_index, _indexPath);
                }

                BinarySerealizer.Save(info, GetStorageFilePath(filename));
            }
        }

        private DataInfo GetFileInternal(string filename)
        {
            return BinarySerealizer.Load<DataInfo>(GetStorageFilePath(filename));
        }

        private string GetStorageFilePath(string filename)
        {
            return Path.Combine(_basePath, filename);
        }
    }
}