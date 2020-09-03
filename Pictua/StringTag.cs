using System;

namespace Pictua
{
    public struct StringTag : ITag
    {
        public string Text { get; }

        public StringTag(string text) => Text = text;

        public override bool Equals(object? obj) => obj is StringTag tag && Text == tag.Text;

        public override int GetHashCode() => HashCode.Combine(Text);

        public static bool operator ==(StringTag left, StringTag right) => left.Equals(right);

        public static bool operator !=(StringTag left, StringTag right) => !(left == right);
    }
}