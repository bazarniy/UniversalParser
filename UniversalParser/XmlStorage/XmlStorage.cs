namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Base;
    using Base.Utilities;

    public class XmlStorage : IDataWriter
    {
        public const string IndexName = "index.xml";
        private readonly IStorageDriver _driver;
        private readonly string _basePath;
        private readonly string _indexPath;

        private XmlStorageIndex _index;

        public XmlStorage(string path, IStorageDriver storageDriver)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (storageDriver == null) throw new ArgumentNullException(nameof(storageDriver));
            PathValidator.ValidateFolderPath(path);

            _basePath = path;
            _driver = storageDriver;
            _indexPath = Path.Combine(_basePath, IndexName);


            Initialize();
        }

        private void Initialize()
        {
            //_driver.DirectoryCreate(_basePath);

            if (_driver.FileExist(_indexPath))
            {
                try
                {
                    //_index = _driver.FileRead<XmlStorageIndex>(_indexPath);
                }
                catch (Exception ex)
                {
                    //TODO: log exception
                }
            }
            _index = _index ?? new XmlStorageIndex();
            ClearWithIndex();
        }

        private void ClearWithIndex()
        {
            var filesToRemove = new List<string>();//_driver.FileEnum(_basePath).ToList();
            var indexFiles = _index.Items.Select(y => y.FileName);

            filesToRemove.Remove(_indexPath);
            filesToRemove.RemoveAll(x => indexFiles.Contains(x));

            foreach (var file in filesToRemove)
            {
                _driver.FileRemove(file);
            }
        }

        public void Write(DataInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            //_driver.FileWrite(info, Path.Combine(_basePath, _driver.GetRandomFileName()));
        }


        public int Count()
        {
            return _index.Items.Count;
        }

        public DataInfo GetFile(string fileName)
        {
            try
            {
                return null;
                //return _driver.FileExist(fileName) ? _driver.FileRead<DataInfo>(Path.Combine(_basePath, fileName)) : null;
            }
            catch (Exception ex)
            {
                //TODO: log exception
                return null;
            }
        }
    }
}