namespace Base.Utilities
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class CloneUtility
    {
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
                return default(T);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }
    }
}