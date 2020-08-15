namespace Pictua.Core
{
    public interface IFileDescriptor
    {
        string FileName { get; }
        string Extension { get; }

        byte[] ContentHash { get; }
    }
}
