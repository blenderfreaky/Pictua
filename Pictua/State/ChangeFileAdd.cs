using System;
using System.Collections.Generic;

namespace Pictua.StateTracking
{
    public struct ChangeFileAdd : IChange
    {
        public ClientIdentity Author { get; }

        public DateTime Time { get; }

        public FileDescriptor File { get; }
        public FileMetadata? NewMetadata { get; }

        public ChangeFileAdd(ClientIdentity author, DateTime time, FileDescriptor file, FileMetadata? newMetadata)
        {
            Author = author;
            Time = time;
            File = file;
            NewMetadata = newMetadata;
        }

        public override bool Equals(object? obj) => obj is ChangeFileAdd add && Equals(add);

        private readonly bool Equals(ChangeFileAdd add) => EqualityComparer<ClientIdentity>.Default.Equals(Author, add.Author) && Time == add.Time && EqualityComparer<FileDescriptor>.Default.Equals(File, add.File) && EqualityComparer<FileMetadata?>.Default.Equals(NewMetadata, add.NewMetadata);

        public override int GetHashCode() => HashCode.Combine(Author, Time, File, NewMetadata);

        public bool Apply(State state)
        {
            return state.Metadata.TryAdd(File, NewMetadata);
        }

        public bool Undo(State state)
        {
            return state.Metadata.Remove(File);
        }

        public static bool operator ==(ChangeFileAdd left, ChangeFileAdd right) => left.Equals(right);

        public static bool operator !=(ChangeFileAdd left, ChangeFileAdd right) => !(left == right);
    }
}