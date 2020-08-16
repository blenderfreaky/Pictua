using System;
using System.Collections.Generic;
using System.IO;

namespace Pictua
{
    public struct FileDescriptor
    {
        public string Extension { get; }

        public byte[] ContentHash { get; }

        public string ContentHashString => BitConverter.ToString(ContentHash).Replace("-", "").ToLowerInvariant();

        public string UniqueName => $"{ContentHashString}.{Extension}";

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

        public override bool Equals(object? obj) => obj is FileDescriptor descriptor && Extension == descriptor.Extension && EqualityComparer<byte[]>.Default.Equals(ContentHash, descriptor.ContentHash) && ContentHashString == descriptor.ContentHashString && UniqueName == descriptor.UniqueName;
        public override int GetHashCode() => HashCode.Combine(Extension, ContentHash, ContentHashString, UniqueName);
    }
}
