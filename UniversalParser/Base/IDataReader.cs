namespace Base
{
    using System.Collections.Generic;

    public interface IDataReader
    {
        IEnumerable<string> GetFileNames();
        DataInfo GetFile(string file);

        IEnumerable<DataInfo> GetInfos();

        int Count();
    }
}