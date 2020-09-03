using System;
using System.Collections.Generic;

namespace Pictua.StateTracking
{
    public struct ChangeFileRemove : IChange, IEquatable<ChangeFileRemove>
    {
        public ClientIdentity Author { get; }

        public DateTime Time { get; }

        public FileDescriptor File { get; }
        public FileMetadata? OldMetadata { get; }

        public ChangeFileRemove(ClientIdentity author, DateTime time, FileDescriptor file, FileMetadata? oldMetadata)
        {
            Author = author;
            Time = time;
            File = file;
            OldMetadata = oldMetadata;
        }

        public override bool Equals(object? obj) => obj is ChangeFileRemove remove && Equals(remove);

        public bool Equals(ChangeFileRemove other) => EqualityComparer<ClientIdentity>.Default.Equals(Author, other.Author) && Time == other.Time && EqualityComparer<FileDescriptor>.Default.Equals(File, other.File) && EqualityComparer<FileMetadata?>.Default.Equals(OldMetadata, other.OldMetadata);

        public override int GetHashCode() => HashCode.Combine(Author, Time, File, OldMetadata);

        public bool Apply(State state)
        {
            return state.Metadata.Remove(File);
        }

        public bool Undo(State state)
        {
            return state.Metadata.TryAdd(File, OldMetadata);
        }

        public static bool operator ==(ChangeFileRemove left, ChangeFileRemove right) => left.Equals(right);

        public static bool operator !=(ChangeFileRemove left, ChangeFileRemove right) => !(left == right);
    }
}