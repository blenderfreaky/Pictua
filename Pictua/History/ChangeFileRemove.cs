using System;
using System.Collections.Generic;

namespace Pictua.HistoryTracking
{
    public struct ChangeFileRemove : IChange
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

        public override bool Equals(object? obj) => obj is ChangeFileRemove remove && EqualityComparer<ClientIdentity>.Default.Equals(Author, remove.Author) && Time == remove.Time && EqualityComparer<FileDescriptor>.Default.Equals(File, remove.File) && EqualityComparer<FileMetadata?>.Default.Equals(OldMetadata, remove.OldMetadata);
        public override int GetHashCode() => HashCode.Combine(Author, Time, File, OldMetadata);

        public bool Apply(State state)
        {
            return state._files.Remove(File);
        }

        public bool Undo(State state)
        {
            return state._files.TryAdd(File, OldMetadata);
        }
    }
}
