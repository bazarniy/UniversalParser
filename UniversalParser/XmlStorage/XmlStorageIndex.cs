namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Base.Serializers;
    using Base.Utilities;

    public sealed class XmlStorageIndex
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
            return driver.Exists(IndexName) 
                ? XmlClassSerializer.Load<StorageIndex>(driver.Read(IndexName)) 
                : new StorageIndex { Items = new List<StorageItem>() };
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