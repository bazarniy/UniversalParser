namespace Networking.WebClient
{
    using System;
    using System.Threading.Tasks;
    using Base;

    public interface IWebClient : IDisposable
    {
        Task<DataInfo> Download(string url, string domain);
    }
}