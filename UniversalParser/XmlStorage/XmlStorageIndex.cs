namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("Index")]
    internal sealed class XmlStorageIndex
    {
        [XmlElement("Item")]
        public List<XmlStorageItem> Items { get; set; }
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