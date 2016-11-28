namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("Index")]
    internal sealed class XmlStorageIndex
    {
        public const string IndexName = "index.xml";
        private IStorageDriver _driver;

        [XmlElement("Item")]
        public List<XmlStorageItem> Items { get; private set; }

        public XmlStorageIndex(IStorageDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            _driver = driver;
        }

        private XmlStorageIndex()
        {
        }

        public static XmlStorageIndex GetIndex(IStorageDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));

            var result = new XmlStorageIndex(driver);
            if (driver.Exists(IndexName))
            {
                //Load
            }
            else
            {
                result.Items = new List<XmlStorageItem>();
            }
            return result;
        }
    }

    [Serializable]
    internal sealed class XmlStorageItem
    {
        [XmlAttribute("filename")]
        public string FileName { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}