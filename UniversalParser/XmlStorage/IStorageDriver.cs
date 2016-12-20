namespace XmlStorage
{
    using System.Collections.Generic;
    using System.IO;

    public interface IStorageDriver
    {
        Stream Write(string name);
        string GetRandomName();
        bool Exists(string name);
        Stream Read(string name);
        IEnumerable<string> Enum();
        void Remove(string name);
        long GetLength(string name);
    }
}
