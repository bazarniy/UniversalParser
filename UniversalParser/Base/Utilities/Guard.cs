namespace Base.Utilities
{
    using System;

    public static class Guard
    {
        public static void ThrowIfNull(this object argumentValue, string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
        }

        public static void ThrowIfEmpty(this string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue)) throw new ArgumentException("param is empty", argumentName);
        }
    }
}