using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlStorage
{
    using System.IO;

    public class StorageDriverFacade:IStorageDriver
    {
        public Stream Write(string name)
        {
            throw new NotImplementedException();
        }

        public string GetRandomName()
        {
            throw new NotImplementedException();
        }

        public bool Exists(string name)
        {
            throw new NotImplementedException();
        }

        public Stream Read(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Enum()
        {
            throw new NotImplementedException();
        }

        public void Remove(string name)
        {
            throw new NotImplementedException();
        }
    }
}
