using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlStorage
{
    using Base;

    public class XmlStorageDriver : IStorageDriver
    {
        public void DirectoryCreate(string path)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExist(string path)
        {
            throw new NotImplementedException();
        }

        public void FileWrite(DataInfo info, string path)
        {
            throw new NotImplementedException();
        }

        public string GetRandomFileName()
        {
            throw new NotImplementedException();
        }
    }
}
