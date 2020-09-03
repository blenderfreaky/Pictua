using System.IO;
using System.Security.Cryptography;

namespace Pictua
{
    public static class FileHashes
    {
        public static byte[] CalculateMD5(string path)
        {
            using var stream = File.OpenRead(path);

            return CalculateMD5(stream);
        }

        public static byte[] CalculateMD5(Stream stream)
        {
            using var md5 = MD5.Create();

            return md5.ComputeHash(stream);
        }
    }
}