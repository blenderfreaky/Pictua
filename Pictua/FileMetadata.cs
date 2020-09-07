using Pictua.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Pictua
{
    public struct FileMetadata
    {
        public DateTime LastUpdated { get; set; }

        [XmlIgnore]
        public IReadOnlyCollection<ITag> Tags { get; set; }

        public List<string> TagStrings
        {
            get { return Tags?.Select(x => x.Text).ToList() ?? new List<string>(); }
            set { Tags = value.Select(x => (ITag)new StringTag(x)).ToList(); }
        }

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