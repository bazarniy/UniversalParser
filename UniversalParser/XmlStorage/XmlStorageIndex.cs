namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Base.Serializers;
    using Base.Utilities;

    public interface IStorageIndex
    {
        void Save();
        int Count();
        void Add(StorageItem item);
        bool Exists(string filename);
    }

    public sealed class XmlStorageIndex : IStorageIndex
    {
        public const string IndexName = "index.xml";
        private readonly IStorageDriver _driver;

        public StorageIndex Items { get; }

        public XmlStorageIndex(IStorageDriver driver)
        {
            driver.ThrowIfNull(nameof(driver));

            _driver = driver;
            Items = GetIndex(_driver);
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
            XmlClassSerializer.Save(Items, _driver.Write(IndexName));
        }

        public int Count()
        {
            return Items.Items.Count;
        }

        public void Add(StorageItem item)
        {
            item.ThrowIfNull(nameof(item));

            Items.Items.Add(item);
        }

        public bool Exists(string filename)
        {
            filename.ThrowIfEmpty(nameof(filename));
            return Items.Items.Any(x => x.FileName == filename);
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