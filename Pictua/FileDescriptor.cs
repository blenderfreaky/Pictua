using System;
using System.IO;
using System.Linq;

namespace Pictua
{
    public struct FileDescriptor : IEquatable<FileDescriptor>
    {
        public string Extension { get; }

        public byte[] ContentHash { get; }

        public string ContentHashString => BitConverter.ToString(ContentHash).Replace("-", "").ToLowerInvariant();

        public string UniqueName => $"{ContentHashString}{Extension}";

        public FileDescriptor(string extension, byte[] contentHash)
        {
            Extension = extension;
            ContentHash = contentHash;
        }

        public FileDescriptor(string filePath)
        {
            Extension = Path.GetExtension(filePath);

            ContentHash = FileHashes.CalculateMD5(filePath);
        }

        public override bool Equals(object? obj) => obj is FileDescriptor descriptor && Equals(descriptor);
        public bool Equals(FileDescriptor other) => Extension == other.Extension && ContentHash.SequenceEqual(other.ContentHash);

        public override int GetHashCode() => HashCode.Combine(Extension, ContentHash, ContentHashString, UniqueName);

        public static bool operator ==(FileDescriptor left, FileDescriptor right) => left.Equals(right);

        public static bool operator !=(FileDescriptor left, FileDescriptor right) => !(left == right);
    }
}
