namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Base;
    using Base.Serializers;
    using Base.Utilities;

    public class XmlStorage : IDataWriter
    {
        private readonly IStorageDriver _driver;
        private readonly string _indexPath;
        private readonly XmlStorageIndex _index;

        public XmlStorage(IStorageDriver storageDriver)
        {
            storageDriver.ThrowIfNull(nameof(storageDriver));

            _driver = storageDriver;
            _index = new XmlStorageIndex(_driver);
        }

        public void Write(DataInfo info)
        {
            info.ThrowIfNull(nameof(info));

            var name = _driver.GetRandomName();
            BinarySerealizer.Save(info, _driver.Write(name));

            _index.Add(new StorageItem {FileName = name, Url = info.Url});
        }


        public int Count()
        {
            return _index.Count();
        }

        public DataInfo GetFile(string fileName)
        {
            fileName.ThrowIfEmpty(nameof(fileName));
            if (_index.Items.Items.Any(x => x.FileName == fileName) && _driver.Exists(fileName))
            {
                return BinarySerealizer.Load<DataInfo>(_driver.Read(fileName));
            }
            return null;
        }
    }
}