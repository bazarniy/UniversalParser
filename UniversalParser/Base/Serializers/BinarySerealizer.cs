namespace Base.Serializers
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class BinarySerealizer
    {
        public static void Save<T>(T source, string path) where T : class
        {
            using (var stream = File.Open(path, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, source);
                stream.Close();
            }
        }

        public static T Load<T>(string path) where T : class
        {
            object source;
            using (var stream = File.Open(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                source = formatter.Deserialize(stream);
            }
            return source as T;
        }

        public static void Save<T>(T source, Stream stream) where T : class
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, source);
            stream.Close();
        }

        public static T Load<T>(Stream stream) where T : class
        {
            if (stream == Stream.Null) return null;

            var formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            var source = formatter.Deserialize(stream);
            stream.Close();
            return source as T;
        }
    }
}