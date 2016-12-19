namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Base.Helpers;
    using Base.Serializers;
    using Base.Utilities;

    public interface IStorageIndex
    {
        void Save();
        int Count();
        void Add(StorageItem item);
        void Remove(StorageItem item);
        StorageItem Get(StorageItem item);
        IEnumerable<StorageItem> Items { get; }
    }

    public sealed class XmlStorageIndex : IStorageIndex
    {
        public const string IndexName = "index.xml";
        private readonly IStorageDriver _driver;
        private readonly object _latch = new object();

        private readonly StorageIndex _items;

        public IEnumerable<StorageItem> Items => _items.Items.ToArray();

        public XmlStorageIndex(IStorageDriver driver)
        {
            driver.ThrowIfNull(nameof(driver));

            _driver = driver;
            _items = GetIndex(_driver);
        }

        private static StorageIndex GetIndex(IStorageDriver driver)
        {
            StorageIndex result = null;
            if (driver.Exists(IndexName))
            {
                try
                {
                    result = XmlClassSerializer.Load<StorageIndex>(driver.Read(IndexName));
                }
                catch (Exception)
                {
                    //TODO: log exception
                }
            }
            return result ?? new StorageIndex {Items = new List<StorageItem>()};
        }

        public void Save()
        {
            XmlClassSerializer.Save(_items, _driver.Write(IndexName));
        }

        public int Count()
        {
            lock(_latch) return _items.Items.Count;
        }

        public void Add(StorageItem item)
        {
            item.ThrowIfNull(nameof(item));

            lock (_latch)
            {
                _items.Items.Add(item);
                Save();
            }
        }

        public void Remove(StorageItem item)
        {
            item.ThrowIfNull(nameof(item));
            lock (_latch)
            {
                _items.Items.RemoveAll(x => x.FileName == item.FileName);
                Save();
            }
        }

        public StorageItem Get(StorageItem item)
        {
            item.ThrowIfNull(nameof(item));
            if (item.FileName.IsEmpty() && item.Url.IsEmpty()) throw new ArgumentException("Item params are empty", nameof(item));
            lock (_latch) return _items.Items.FirstOrDefault(x => x.FileName == item.FileName || x.Url == item.Url);
        }
    }

    [Serializable]
    [XmlRoot("Index")]
    public sealed class StorageIndex
    {
        [XmlElement("Item")]
        public List<StorageItem> Items { get; set; }
    }

    [Serializable]
    public sealed class StorageItem
    {
        [XmlAttribute("filename")]
        public string FileName { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}