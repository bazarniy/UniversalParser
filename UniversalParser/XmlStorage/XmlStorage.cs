namespace XmlStorage
{
    using System;
    using Base;
    using Base.Utilities;

    public class XmlStorage:IDataWriter
    {
        private readonly IStorageDriver _driver;
        private readonly string _basePath;

        public XmlStorage(string path, IStorageDriver storageDriver)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (storageDriver == null) throw new ArgumentNullException(nameof(storageDriver));
            PathValidator.ValidateFolderPath(path);

            _basePath = path;
            _driver = storageDriver;
            _driver.DirectoryCreate(path);
        }

        public void Write(DataInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            _driver.FileWrite(info, _driver.GetRandomFileName());
        }
    }
}