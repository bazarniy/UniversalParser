namespace Base.Helpers
{
    public static class UrlExtentions
    {
        public static Url Fix(this Url url)
        {
            if (url == null) return null;
            while (url.Path.EndsWith("//"))
            {
                url.Path = url.Path.Substring(0, url.Path.Length - 1);
            }

            return IsMailtoError(url.Path) ? null : url;
        }

        private static bool IsMailtoError(string path)
        {
            return path.Contains("mailto") && path.Contains("@");
        }
    }
}
