namespace Pictua.HistoryTracking
{
    using Pictua;
    using System;
    using System.Collections.Generic;

    public struct ChangeMetadata : IChange
    {
        public IClientIdentity Author { get; }
        public DateTime Time { get; }

        public FileDescriptor Target { get; }
        public FileMetadata NewMetadata { get; }

        public ChangeMetadata(IClientIdentity author, DateTime time, FileDescriptor target, FileMetadata newMetadata)
        {
            Author = author;
            Time = time;
            Target = target;
            NewMetadata = newMetadata;
        }

        public IChange WithTime(DateTime dateTime) => new ChangeMetadata(Author, dateTime, Target, NewMetadata);
        public override bool Equals(object? obj) => obj is ChangeMetadata metadata && EqualityComparer<IClientIdentity>.Default.Equals(Author, metadata.Author) && Time == metadata.Time && EqualityComparer<FileDescriptor>.Default.Equals(Target, metadata.Target) && EqualityComparer<FileMetadata>.Default.Equals(NewMetadata, metadata.NewMetadata);
        public override int GetHashCode() => HashCode.Combine(Author, Time, Target, NewMetadata);
    }
}
