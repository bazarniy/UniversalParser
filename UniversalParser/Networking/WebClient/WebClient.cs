namespace Networking.WebClient
{
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Base;
    using Base.Helpers;

    public class WebClient : IWebClient
    {
        private readonly System.Net.WebClient _client = new System.Net.WebClient();

        public async Task<WebResult> Download(Url url)
        {
            //TODO: проверять content type перед закачкой
            var result = new WebResult {Url = url};
            try
            {
                var rawdata = await _client.DownloadDataTaskAsync(url.ToString());
                result.Data = Encoding
                    .GetEncoding(GetCharset() ?? Encoding.UTF8.WebName)
                    .GetString(rawdata);
                result.ErrorCode = 200;
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                result.ErrorCode = GetStatusCode(ex);
            }

            return result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static int GetStatusCode(WebException ex)
        {
            return (int) ((HttpWebResponse) ex.Response).StatusCode;
        }

        private string GetCharset()
        {
            return new ContentType(_client.ResponseHeaders[HttpResponseHeader.ContentType]).CharSet;
        }
    }
}
