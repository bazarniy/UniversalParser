namespace Networking.DataWriter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Base;
    using Base.Serializers;

    public class DataWriter : IDataWriter
    {
        private const string IndexName = "index.xml";
        private readonly DataWriterIndex _index;
        private readonly string _indexPath;

        private readonly object _latch = new object();

        private readonly string _path;

        public DataWriter(string path)
        {
            _path = path;
            _indexPath = Path.Combine(_path, IndexName);

            if (File.Exists(_indexPath))
                _index = XmlClassSerializer.Load<DataWriterIndex>(_indexPath);
            else
                Directory.CreateDirectory(_path);
        }

        public IEnumerable<string> ParsedLinks
        {
            get
            {
                lock (_latch)
                {
                    return _index.Items.Select(x => x.Url).ToArray();
                }
            }
        }

        public void Write(DataInfo info)
        {
            BinarySerealizer.Save(info, GetPath(info));
        }

        private string GetPath(DataInfo info)
        {
            lock (_latch)
            {
                var item = _index.Items.FirstOrDefault(x => x.Url == info.Url);
                if (item != null)
                    return item.FileName;
                var path = Path.Combine(_path, Path.GetTempFileName());
                _index.Items.Add(new DataWriterItem {FileName = path, Url = info.Url});
                XmlClassSerializer.Save(_index, _indexPath);
                return path;
            }
        }
    }
}