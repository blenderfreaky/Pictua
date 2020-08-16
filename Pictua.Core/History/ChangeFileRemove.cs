namespace Pictua.HistoryTracking
{
    using Pictua;
    using System;
    using System.Collections.Generic;

    public struct ChangeFileRemove : IChange
    {
        public IClientIdentity Author { get; }
        public DateTime Time { get; }

        public FileDescriptor File { get; }

        public ChangeFileRemove(IClientIdentity author, DateTime time, FileDescriptor file)
        {
            Author = author;
            Time = time;
            File = file;
        }

        public IChange WithTime(DateTime dateTime) => new ChangeFileRemove(Author, dateTime, File);
        public override bool Equals(object? obj) => obj is ChangeFileRemove remove && EqualityComparer<IClientIdentity>.Default.Equals(Author, remove.Author) && Time == remove.Time && EqualityComparer<FileDescriptor>.Default.Equals(File, remove.File);
        public override int GetHashCode() => HashCode.Combine(Author, Time, File);
    }
}
