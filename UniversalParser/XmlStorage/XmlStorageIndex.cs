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
        public List<XmlStorageItem> Items { get; set; }

        public XmlStorageIndex(IStorageDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            _driver = driver;
            Items = new List<XmlStorageItem>();
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
                result = new XmlStorageIndex(driver) {Items = new List<XmlStorageItem>()};
            }
            return result;
        }
    }

    [Serializable]
    public sealed class XmlStorageItem
    {
        [XmlAttribute("filename")]
        public string FileName { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}