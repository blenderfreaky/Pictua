namespace Pictua.Core
{
    using System.IO;
    using System.Security.Cryptography;

    public static class FileHashes
    {
        public static byte[] CalculateMD5(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);

            return md5.ComputeHash(stream);
        }
    }
}
