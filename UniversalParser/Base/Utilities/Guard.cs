namespace Base.Utilities
{
    using System;
    using Helpers;

    public static class Guard
    {
        public static void ThrowIfNull(this object argumentValue, string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
        }

        public static void ThrowIfEmpty(this string argumentValue, string argumentName)
        {
            if (argumentValue.IsEmpty()) throw new ArgumentException("param is empty", argumentName);
        }

        public static void ThrowIfEmpty(this string argumentValue, string argumentName, string error)
        {
            if (argumentValue.IsEmpty()) throw new ArgumentException(error, argumentName);
        }
    }
}