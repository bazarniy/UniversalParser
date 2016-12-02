namespace XmlStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using Base.Utilities;

    public class StorageDriverFacade:IStorageDriver
    {
        private readonly string _extention;
        private IStorageDriver _driver;

        public StorageDriverFacade(string extention, IStorageDriver driver)
        {
            driver.ThrowIfNull(nameof(driver));
            if (extention.Length > 3) throw new ArgumentException("extention is too long", nameof(extention));
            PathValidator.ValidateExtention(extention);

            _extention = string.IsNullOrWhiteSpace(extention) ? "" : "." + extention;
            _driver = driver;
        }

        public string GetRandomName()
        {
            return Guid.NewGuid().ToString("N") + _extention;
        }

        public Stream Write(string name)
        {
            return _driver.Write(GetNameWithExtention(name));
        }

        public bool Exists(string name)
        {
            return _driver.Exists(GetNameWithExtention(name));
        }

        public Stream Read(string name)
        {
            return _driver.Read(GetNameWithExtention(name));
        }

        public void Remove(string name)
        {
            _driver.Remove(GetNameWithExtention(name));
        }

        public IEnumerable<string> Enum()
        {
            return _driver.Enum().Where(EndsWithExtention);
        }

        private bool EndsWithExtention(string x)
        {
            return string.IsNullOrEmpty(_extention) ? !x.Contains(".") : x.EndsWith(_extention);
        }

        private string GetNameWithExtention(string name)
        {
            return name.ToUpperInvariant().Contains(_extention.ToUpperInvariant()) ? name : name + _extention;
        }
    }
}
