namespace Base
{
    public interface IDataWriter
    {
        void Write<T>(T info, string url) where T : class;
    }
}