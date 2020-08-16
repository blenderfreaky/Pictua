namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;

    public struct DiffMetadata : IDiff
    {
        public IClientIdentity Author { get; }
        public DateTime Time { get; }
        public IError? Error { get; }

    }

    public struct DiffRemove : IDiff
    {
        public IClientIdentity Author { get; }
        public DateTime Time { get; }
        public IError? Error { get; }
        public FileDescriptor File { get; }

        public DiffRemove(IClientIdentity author, DateTime time, IError? error, FileDescriptor file)
        {
            Author = author;
            Time = time;
            Error = error;
            File = file;
        }

        public override bool Equals(object? obj) => obj is DiffRemove remove && EqualityComparer<IClientIdentity>.Default.Equals(Author, remove.Author) && Time == remove.Time && EqualityComparer<IError?>.Default.Equals(Error, remove.Error) && EqualityComparer<FileDescriptor>.Default.Equals(File, remove.File);
        public override int GetHashCode() => HashCode.Combine(Author, Time, Error, File);

        public static bool operator ==(DiffRemove? left, DiffRemove? right) => EqualityComparer<DiffRemove>.Default.Equals(left, right);
        public static bool operator !=(DiffRemove? left, DiffRemove? right) => !(left == right);
    }
}
