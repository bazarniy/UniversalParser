namespace Networking.WebClient
{
    using System;
    using System.Threading.Tasks;
    using Base;

    public interface IWebClient : IDisposable
    {
        Task<WebResult> Download(Url url);
    }
}