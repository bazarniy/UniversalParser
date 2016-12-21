namespace Base.Utilities
{
    using System.IO;
    using System.Linq;

    public static class FileStreamEquals
    {
        public static bool Equals(Stream stream1, Stream stream2)
        {
            if (stream1 == Stream.Null && stream2 == Stream.Null) return true;
            if (stream1 == null && stream2 == null) return true;
            if (stream1 == Stream.Null || stream2 == Stream.Null) return false;
            if (stream1 == null || stream2 == null) return false;

            const int bufferSize = 2048;
            byte[] buffer1 = new byte[bufferSize]; //buffer size
            byte[] buffer2 = new byte[bufferSize];

            stream1.Position = 0;
            stream2.Position = 0;

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                    return false;

                if (count1 == 0)
                    return true;

                // You might replace the following with an efficient "memcmp"
                if (!buffer1.Take(count1).SequenceEqual(buffer2.Take(count2)))
                    return false;
            }
        }
    }
}
