namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Base.Serializers;

    [Serializable]
    [XmlRoot("Index")]
    public sealed class XmlStorageIndex
    {
        public const string IndexName = "index.xml";
        private IStorageDriver _driver;

        [XmlElement("Item")]
        private List<StorageItem> Items { get; set; }

        public XmlStorageIndex(IStorageDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            _driver = driver;
            Items = new List<StorageItem>();
        }

        private XmlStorageIndex()
        {
        }

        public void Save()
        {
            XmlClassSerializer.Save(this, _driver.Write(IndexName));
        }

        public static XmlStorageIndex GetIndex(IStorageDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));

            XmlStorageIndex result;
            if (driver.Exists(IndexName))
            {
                result = XmlClassSerializer.Load<XmlStorageIndex>(driver.Read(IndexName));
                result._driver = driver;
            }
            else
            {
                result = new XmlStorageIndex(driver) {Items = new List<StorageItem>()};
            }
            return result;
        }

        public int Count()
        {
            return Items.Count;
        }

        public void Add(StorageItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }
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