namespace Base.Helpers
{
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using Base;

    public static class WebClientHelpers
    {
        public static DataInfo Download(this WebClient client, string url)
        {
            var result = new DataInfo(url);
            byte[] rawdata;
            try
            {
                rawdata = client.DownloadData(url);
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
            {
                result.Code = ex.GetStatusCode();
                return result;
            }

            result.Data = Encoding
                .GetEncoding(client.GetCharset())
                .GetString(rawdata);
            return result;
        }

        private static string GetCharset(this WebClient client)
        {
            return new ContentType(client.ResponseHeaders[HttpResponseHeader.ContentType]).CharSet;
        }

        private static int GetStatusCode(this WebException ex)
        {
            return (int) ((HttpWebResponse) ex.Response).StatusCode;
        }
    }
}
