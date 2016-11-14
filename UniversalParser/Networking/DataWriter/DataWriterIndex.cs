namespace Networking.DataWriter
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("Index")]
    public sealed class DataWriterIndex
    {
        [XmlElement("Item")]
        public List<DataWriterItem> Items { get; set; }
    }

    [Serializable]
    public sealed class DataWriterItem
    {
        [XmlAttribute("filename")]
        public string FileName { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}