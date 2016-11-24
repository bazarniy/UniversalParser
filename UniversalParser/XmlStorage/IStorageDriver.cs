using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlStorage
{
    using System.IO;
    using Base;

    public interface IStorageDriver
    {
        void DirectoryCreate(string path);
        bool DirectoryExist(string path);

        void FileWrite(DataInfo info, string path);

        string GetRandomFileName();
    }
}
