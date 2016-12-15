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

        public async Task<DataInfo> Download(Url url)
        {
            var result = new DataInfo(url.ToString());
            byte[] rawdata;
            try
            {
                rawdata = await _client.DownloadDataTaskAsync(url.ToString());
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                result.Code = GetStatusCode(ex);
                return result;
            }

            result.Data = Encoding
                .GetEncoding(GetCharset())
                .GetString(rawdata);

            result.Links = HtmlHelpers.GetLinks(result.Data, url).ToArray();

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
