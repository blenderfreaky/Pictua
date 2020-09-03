using System;
using System.Collections.Generic;

namespace Pictua
{
    public struct FileMetadata
    {
        public DateTime LastUpdated { get; set; }
        public IReadOnlyCollection<ITag> Tags { get; set; }

        public FileMetadata(DateTime lastUpdated, IReadOnlyCollection<ITag> tags)
        {
            LastUpdated = lastUpdated;
            Tags = tags;
        }

        public override bool Equals(object? obj) => obj is FileMetadata metadata && LastUpdated == metadata.LastUpdated && EqualityComparer<IReadOnlyCollection<ITag>>.Default.Equals(Tags, metadata.Tags);

        public override int GetHashCode() => HashCode.Combine(LastUpdated, Tags);

        public static bool operator ==(FileMetadata left, FileMetadata right) => left.Equals(right);

        public static bool operator !=(FileMetadata left, FileMetadata right) => !(left == right);
    }
}