namespace Base
{
    using System;
    using System.Collections.Generic;
    using Helpers;

    [Serializable]
    public class DataInfo
    {
        public readonly string Url;
        public int Code;
        public string Data;
        public IEnumerable<Url> Links => HtmlHelpers.GetAllLinks(Data, Url);

        public DataInfo(string url, string data = null)
        {
            Url = url;
            Data = data;
            Code = 200;
        }
    }
}