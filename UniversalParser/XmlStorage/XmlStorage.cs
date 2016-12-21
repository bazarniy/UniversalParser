namespace XmlStorage
{
    using System.Collections.Generic;
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
            foreach (var file in files.Where(x => _index.Get(new StorageItem {FileName = x}) == null))
            {
                _driver.Remove(file);
            }

            foreach (var file in _index.Items.Where(x => !files.Contains(x.FileName)).ToArray())
            {
                _index.Remove(file);
            }
        }

        public void Write<T>(T info, string url) where T: class
        {
            info.ThrowIfNull(nameof(info));
            url.ThrowIfEmpty(nameof(url));

            var name = _driver.GetRandomName();
            BinarySerealizer.Save(info, _driver.Write(name));

            _index.Add(new StorageItem {FileName = name, Url = url});
        }


        public int Count()
        {
            return _index.Count();
        }

        public T Read<T>(string fileName) where T : class
        {
            fileName.ThrowIfEmpty(nameof(fileName));
            return Read<T>(new StorageItem { FileName = fileName });
        }

        private T Read<T>(StorageItem item) where T : class
        {
            item = _index.Get(item);
            if (item != null && _driver.Exists(item.FileName))
            {
                return BinarySerealizer.Load<T>(_driver.Read(item.FileName));
            }
            return null;
        }

        public IEnumerable<string> Enum()
        {
            return _index.Items.Select(x => x.Url);
        }

        public static XmlStorage GetStorage(string path, string extention)
        {
            path.ThrowIfEmpty(nameof(path));

            var driver = new DiskDriver(path);
            return new XmlStorage(new StorageDriverFacade(extention, driver), new XmlStorageIndex(new StorageDriverFacade("xml", driver)));
        }

        public void Deduplication()
        {
            long iterations = 0;
            var changedItems = new List<int>();
            var items = _index.Items
                .Where(item => _driver.Exists(item.FileName))
                .ToDictionary(item => item, item => _driver.GetLength(item.FileName));

            var length = items.Count;

            for (int i = 0; i < length; i++)
            {
                if (changedItems.Contains(i)) continue;
                var item1 = items.ElementAt(i);

                using (var s1 = _driver.Read(item1.Key.FileName))
                {
                    for (int j = i + 1; j < length; j++)
                    {
                        iterations++;
                        if (changedItems.Contains(j)) continue;

                        var item2 = items.ElementAt(j);

                        if (item1.Value != item2.Value) continue;

                        using (var s2 = _driver.Read(item2.Key.FileName))
                        {
                            if (!FileStreamEquals.Equals(s1, s2)) continue;
                        }

                        _driver.Remove(item2.Key.FileName);
                        item2.Key.FileName = item1.Key.FileName;
                        
                        changedItems.Add(j);
                    }
                }
                _index.Save();
            }
            _index.Save();
        }

        public void Deduplication1()
        {
            long iterations = 0;
            var changedFiles = new List<string>();
            foreach (var item in _index.Items)
            {
                if (changedFiles.Contains(item.FileName) || !_driver.Exists(item.FileName)) continue;

                var length = _driver.GetLength(item.FileName);
                using (var s1 = _driver.Read(item.FileName))
                {
                    foreach (var item2 in _index.Items.Where(x => x.FileName != item.FileName))
                    {
                        iterations++;
                        if (!_driver.Exists(item2.FileName) || _driver.GetLength(item2.FileName) != length) continue;

                        using (var s2 = _driver.Read(item2.FileName))
                        {
                            if (!FileStreamEquals.Equals(s1, s2)) continue;

                            s2.Close();
                            _driver.Remove(item2.FileName);
                            item2.FileName = item.FileName;
                            _index.Save();

                            changedFiles.Add(item.FileName);
                        }
                    }
                }
            }
        }


    }
}