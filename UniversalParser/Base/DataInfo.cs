namespace Base
{
    using System;

    [Serializable]
    public class DataInfo
    {
        public readonly string Url;
        public int Code;
        public string Data;
        public string[] Links;

        public DataInfo(string url)
        {
            Url = url;
            Data = null;
            Code = 200;
        }
    }
}