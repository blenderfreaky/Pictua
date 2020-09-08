using System;
using System.IO;

namespace Pictua
{
    public struct FileDescriptor : IEquatable<FileDescriptor>
    {
        public string Extension { get; set; }

        public string UniqueName { get; set; }

        public FileDescriptor(string extension, byte[] contentHash)
        {
            Extension = extension;
            UniqueName = BitConverter.ToString(contentHash).Replace("-", "").ToLowerInvariant() + Extension;
        }

        public FileDescriptor(string filePath) : this(Path.GetExtension(filePath), FileHashes.CalculateMD5(filePath)) { }

        public override bool Equals(object? obj) => obj is FileDescriptor descriptor && Equals(descriptor);

        public bool Equals(FileDescriptor other) => Extension == other.Extension && UniqueName == other.UniqueName;

        public override int GetHashCode() => HashCode.Combine(Extension, UniqueName);

        public static bool operator ==(FileDescriptor left, FileDescriptor right) => left.Equals(right);

        public static bool operator !=(FileDescriptor left, FileDescriptor right) => !(left == right);
    }
}