namespace XmlStorage
{
    using System.Linq;
    using Base;
    using Base.Serializers;
    using Base.Utilities;

    public class XmlStorage : IDataWriter
    {
        private readonly IStorageDriver _driver;
        private readonly IStorageIndex _index;

        public XmlStorage(IStorageDriver storageDriver, IStorageIndex index)
        {
            storageDriver.ThrowIfNull(nameof(storageDriver));
            index.ThrowIfNull(nameof(index));

            _driver = storageDriver;
            _index = index;

            ClearFilesAndIndex();
        }

        private void ClearFilesAndIndex()
        {
            var files = _driver.Enum().ToArray();
            foreach (var file in files.Where(x => !_index.Exists(x)))
            {
                _driver.Remove(file);
            }

            foreach (var file in _index.Items.Where(x => !files.Contains(x.FileName)).ToArray())
            {
                _index.Remove(file);
            }
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
            if (_index.Exists(fileName) && _driver.Exists(fileName))
            {
                return BinarySerealizer.Load<DataInfo>(_driver.Read(fileName));
            }
            return null;
        }

        public static XmlStorage GetStorage(string path, string extention)
        {
            path.ThrowIfEmpty(nameof(path));

            var driver = new DiskDriver(path);
            return new XmlStorage(new StorageDriverFacade(extention, driver), new XmlStorageIndex(new StorageDriverFacade("xml", driver)));
        }
    }
}