namespace Base.Serializers
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public static class XmlClassSerializer
    {
        public static void Save<T>(T source, string path) where T : class
        {
            var writer = new XmlSerializer(typeof(T));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var stream = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.Serialize(stream, source, namespaces);
            }
        }

        public static T Load<T>(string path) where T : class
        {
            object source;
            var reader = new XmlSerializer(typeof(T));
            using (var stream = new StreamReader(path, Encoding.UTF8))
            {
                source = reader.Deserialize(stream);
            }
            return source as T;
        }

        public static void Save<T>(T source, Stream stream) where T : class
        {
            var writer = new XmlSerializer(typeof(T));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Serialize(streamWriter, source, namespaces);
            }
        }

        public static T Load<T>(Stream stream) where T : class
        {
            if (stream == Stream.Null) return null;

            object source;
            var reader = new XmlSerializer(typeof(T));
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                source = reader.Deserialize(streamReader);
            }
            return source as T;
        }
    }
}