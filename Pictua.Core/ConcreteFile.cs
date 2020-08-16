namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;

    public struct ConcreteFile
    {
        public string LocalPath { get; }
        public FileDescriptor FileDescriptor { get; }
        public FileMetadata Metadata { get; set; }

        public ConcreteFile(string localPath, FileDescriptor fileDescriptor, FileMetadata metadata)
        {
            LocalPath = localPath;
            FileDescriptor = fileDescriptor;
            Metadata = metadata;
        }

        public override bool Equals(object? obj) => obj is ConcreteFile file && LocalPath == file.LocalPath && EqualityComparer<FileDescriptor>.Default.Equals(FileDescriptor, file.FileDescriptor) && EqualityComparer<FileMetadata>.Default.Equals(Metadata, file.Metadata);
        public override int GetHashCode() => HashCode.Combine(LocalPath, FileDescriptor, Metadata);

        public static bool operator ==(ConcreteFile left, ConcreteFile right) => left.Equals(right);
        public static bool operator !=(ConcreteFile left, ConcreteFile right) => !(left == right);
    }
}
