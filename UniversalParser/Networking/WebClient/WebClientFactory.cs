namespace Networking.WebClient
{
    public class WebClientFactory : IWebClientFactory
    {
        public IWebClient Create()
        {
            return new WebClient();
        }
    }
}