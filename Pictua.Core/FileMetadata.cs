using System;
using System.Collections.Generic;

namespace Pictua
{
    public struct FileMetadata
    {
        public DateTime LastUpdated { get; }
        public IReadOnlyCollection<ITag> Tags { get; }
        public decimal Rating { get; }

        public FileMetadata(DateTime lastUpdated, IReadOnlyCollection<ITag> tags, decimal rating)
        {
            LastUpdated = lastUpdated;
            Tags = tags;
            Rating = rating;
        }

        public override bool Equals(object? obj) => obj is FileMetadata metadata && LastUpdated == metadata.LastUpdated && EqualityComparer<IReadOnlyCollection<ITag>>.Default.Equals(Tags, metadata.Tags) && Rating == metadata.Rating;
        public override int GetHashCode() => HashCode.Combine(LastUpdated, Tags, Rating);

        public static bool operator ==(FileMetadata left, FileMetadata right) => left.Equals(right);
        public static bool operator !=(FileMetadata left, FileMetadata right) => !(left == right);
    }
}
