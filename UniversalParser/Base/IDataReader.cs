namespace Base
{
    using System.Collections.Generic;

    public interface IDataReader
    {
        IEnumerable<DataInfo> GetInfos();

        int Count();
    }
}