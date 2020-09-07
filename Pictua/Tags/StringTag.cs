using System;
using System.Linq;

namespace Pictua.Tags
{
    public struct StringTag : ITag
    {
        public string[] SubTags { get; }

        public string Text => string.Join("/", SubTags);

        public StringTag(params string[] subTags) => SubTags = subTags;
        public StringTag(string text) : this(text.Split('/')) { }

        public override bool Equals(object? obj) => obj is StringTag tag && SubTags.SequenceEqual(tag.SubTags);

        public override int GetHashCode() => HashCode.Combine(SubTags);

        public static bool operator ==(StringTag left, StringTag right) => left.Equals(right);

        public static bool operator !=(StringTag left, StringTag right) => !(left == right);
    }
}