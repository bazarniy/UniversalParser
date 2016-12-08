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

        /*public DataInfo Download(string url, string domain)
        {
            var result = new DataInfo(url);
            byte[] rawdata;
            try
            {
                rawdata = _client.DownloadData(url);
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                result.Code = GetStatusCode(ex);
                return result;
            }

            result.Data = Encoding
                .GetEncoding(GetCharset())
                .GetString(rawdata);

            result.Links = HtmlHelpers.GetLinks(result.Data, result.Url, domain).ToArray();

            return result;
        }*/

        /*public DataInfo Download(string url)
        {
            return Download(url, UrlHelpers.GetDomain(url));
        }*/

        public async Task<DataInfo> Download(string url, string domain)
        {
            var result = new DataInfo(url);
            byte[] rawdata;
            try
            {
                rawdata = await _client.DownloadDataTaskAsync(url);
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                result.Code = GetStatusCode(ex);
                return result;
            }

            result.Data = Encoding
                .GetEncoding(GetCharset())
                .GetString(rawdata);

            result.Links = HtmlHelpers.GetLinks(result.Data, result.Url, domain).ToArray();

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
