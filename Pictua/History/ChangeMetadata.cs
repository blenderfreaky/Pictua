using System;
using System.Collections.Generic;

namespace Pictua.HistoryTracking
{
    public struct ChangeMetadata : IChange, IEquatable<ChangeMetadata>
    {
        public ClientIdentity Author { get; }

        public DateTime Time { get; }

        public FileDescriptor Target { get; }
        public FileMetadata? OldMetadata { get; }
        public FileMetadata? NewMetadata { get; }

        public ChangeMetadata(ClientIdentity author, DateTime time, FileDescriptor target, FileMetadata? oldMetadata, FileMetadata? newMetadata)
        {
            Author = author;
            Time = time;
            Target = target;
            OldMetadata = oldMetadata;
            NewMetadata = newMetadata;
        }

        public override bool Equals(object? obj) => obj is ChangeMetadata metadata && EqualityComparer<ClientIdentity>.Default.Equals(Author, metadata.Author) && Time == metadata.Time && EqualityComparer<FileDescriptor>.Default.Equals(Target, metadata.Target) && EqualityComparer<FileMetadata?>.Default.Equals(OldMetadata, metadata.OldMetadata) && EqualityComparer<FileMetadata?>.Default.Equals(NewMetadata, metadata.NewMetadata);
        public bool Equals(ChangeMetadata other) => EqualityComparer<ClientIdentity>.Default.Equals(Author, other.Author) && Time == other.Time && EqualityComparer<FileDescriptor>.Default.Equals(Target, other.Target) && EqualityComparer<FileMetadata?>.Default.Equals(OldMetadata, other.OldMetadata) && EqualityComparer<FileMetadata?>.Default.Equals(NewMetadata, other.NewMetadata);

        public override int GetHashCode() => HashCode.Combine(Author, Time, Target, OldMetadata, NewMetadata);

        public bool Apply(State state)
        {
            if (!state.HasFile(Target)) return false;
            state._files[Target] = NewMetadata;
            return true;
        }

        public bool Undo(State state)
        {
            if (!state.HasFile(Target)) return false;
            state._files[Target] = OldMetadata;
            return true;
        }

        public static bool operator ==(ChangeMetadata left, ChangeMetadata right) => left.Equals(right);

        public static bool operator !=(ChangeMetadata left, ChangeMetadata right) => !(left == right);
    }
}
