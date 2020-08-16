namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;

    public struct DiffAdd : IDiff
    {
        public IClientIdentity Author { get; }
        public DateTime Time { get; }
        public IError? Error { get; }
        public FileDescriptor File { get; }

        public DiffAdd(IClientIdentity author, DateTime time, IError? error, FileDescriptor file)
        {
            Author = author;
            Time = time;
            Error = error;
            File = file;
        }

        public override bool Equals(object? obj) => obj is DiffAdd add && EqualityComparer<IClientIdentity>.Default.Equals(Author, add.Author) && Time == add.Time && EqualityComparer<IError?>.Default.Equals(Error, add.Error) && EqualityComparer<FileDescriptor>.Default.Equals(File, add.File);
        public override int GetHashCode() => HashCode.Combine(Author, Time, Error, File);

        public static bool operator ==(DiffAdd left, DiffAdd right) => left.Equals(right);
        public static bool operator !=(DiffAdd left, DiffAdd right) => !(left == right);
    }
}
