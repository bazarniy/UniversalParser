namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("Index")]
    internal sealed class XmlStorageIndex
    {
        private List<XmlStorageItem> _items;

        [XmlElement("Item")]
        public List<XmlStorageItem> Items => _items ?? (_items = new List<XmlStorageItem>());
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