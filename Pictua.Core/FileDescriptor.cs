namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public struct FileDescriptor
    {
        public string FileName { get; }
        public string Extension { get; }

        public byte[] ContentHash { get; }

        public FileDescriptor(string fileName, string extension, byte[] contentHash) : this(fileName)
        {
            Extension = extension;
            ContentHash = contentHash;
        }

        public FileDescriptor(string filePath)
        {
            FileName = Path.GetFileNameWithoutExtension(filePath);
            Extension = Path.GetExtension(filePath);

            ContentHash = FileHashes.CalculateMD5(filePath);
        }

        public override bool Equals(object obj) => obj is FileDescriptor descriptor && FileName == descriptor.FileName && Extension == descriptor.Extension && EqualityComparer<byte[]>.Default.Equals(ContentHash, descriptor.ContentHash);
        public override int GetHashCode() => HashCode.Combine(FileName, Extension, ContentHash);

        public static bool operator ==(FileDescriptor left, FileDescriptor right) => left.Equals(right);

        public static bool operator !=(FileDescriptor left, FileDescriptor right) => !(left == right);
    }
}
