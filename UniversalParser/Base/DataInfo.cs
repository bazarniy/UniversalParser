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
        public IEnumerable<Url> Links => HtmlHelpers.GetLinks(Data, Url);

        public DataInfo(string url)
        {
            Url = url;
            Data = null;
            Code = 200;
        }
    }
}