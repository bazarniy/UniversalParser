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
        string GetRandomFileName();
        bool FileExist(string path);
        IEnumerable<string> FileEnum();
        void FileRemove(string path);
    }
}
