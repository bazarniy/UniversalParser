using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{
    using System.Data;
    using System.IO;
    using System.Xml.Serialization;
    using Base.Helpers;
    using Extraction.Common;
    using HtmlAgilityPack;
    using Networking;
    using Networking.WebClient;
    using XmlStorage;

    class Program
    {
        static void Main(string[] args)
        {
            var storage = XmlStorage.GetStorage("iekru", "dwl");
            var loader = new DomainLoader(new WebClientFactory(), storage, "https://www.iek.ru");
            var task = loader.Download();
            task.Wait();
            var results = loader.GetResults();
            File.WriteAllLines("result.txt", results.Select(x=>$"{x.Key} - {x.Value?.ToString()}"));
        }
    }

    
}
