namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;

    public class MetaDataUpdate
    {
        public FileDescriptor FileDescriptor { get; }
        public FileMetadata Metadata { get; }

        public ICollection<IClientIdentity> Owners { get; }

        public MetaDataUpdate(FileDescriptor fileDescriptor, FileMetadata metadata, ICollection<IClientIdentity> owners)
        {
            FileDescriptor = fileDescriptor;
            Metadata = metadata;
            Owners = owners;
        }

        public override bool Equals(object? obj) => obj is MetaDataUpdate update && EqualityComparer<FileDescriptor>.Default.Equals(FileDescriptor, update.FileDescriptor) && EqualityComparer<FileMetadata>.Default.Equals(Metadata, update.Metadata) && EqualityComparer<ICollection<IClientIdentity>>.Default.Equals(Owners, update.Owners);
        public override int GetHashCode() => HashCode.Combine(FileDescriptor, Metadata, Owners);

        public static bool operator ==(MetaDataUpdate? left, MetaDataUpdate? right) => EqualityComparer<MetaDataUpdate>.Default.Equals(left, right);
        public static bool operator !=(MetaDataUpdate? left, MetaDataUpdate? right) => !(left == right);
    }
}
