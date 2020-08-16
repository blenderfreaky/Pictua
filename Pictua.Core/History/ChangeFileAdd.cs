namespace Pictua.HistoryTracking
{
    using Pictua;
    using System;
    using System.Collections.Generic;

    public struct ChangeFileAdd : IChange
    {
        public IClientIdentity Author { get; }

        public DateTime Time { get; }

        public FileDescriptor File { get; }

        public ChangeFileAdd(IClientIdentity author, DateTime time, FileDescriptor file)
        {
            Author = author;
            Time = time;
            File = file;
        }

        public IChange WithTime(DateTime dateTime) => new ChangeFileRemove(Author, dateTime, File);
        public override bool Equals(object? obj) => obj is ChangeFileAdd add && EqualityComparer<IClientIdentity>.Default.Equals(Author, add.Author) && Time == add.Time && EqualityComparer<FileDescriptor>.Default.Equals(File, add.File);
        public override int GetHashCode() => HashCode.Combine(Author, Time, File);
    }
}
