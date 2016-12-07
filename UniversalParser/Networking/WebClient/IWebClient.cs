namespace Networking.WebClient
{
    using System;
    using Base;

    public interface IWebClient : IDisposable
    {
        DataInfo Download(string url);
        DataInfo Download(string url, string domain);
    }
}