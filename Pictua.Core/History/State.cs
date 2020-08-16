using System;
using System.Collections.Generic;

namespace Pictua.HistoryTracking
{
    public class State
    {
        internal readonly Dictionary<FileDescriptor, FileMetadata?> _files;

        public FileMetadata? GetMetadata(FileDescriptor fileDescriptor) => _files.TryGetValue(fileDescriptor, out var metadata) ? metadata : null;

        public bool HasFile(FileDescriptor fileDescriptor) => _files.ContainsKey(fileDescriptor);

        public bool HadErrors { get; protected set; } = false;

        public State()
        {
            _files = new Dictionary<FileDescriptor, FileMetadata?>();
        }

        public State(History history) : this()
        {
            foreach (var change in history) Apply(change);
        }

        public bool Apply(IChange change)
        {
            var valid = change.Apply(this);
            HadErrors |= valid;
            return valid;
        }

        public bool Undo(IChange change)
        {
            var valid = change.Undo(this);
            HadErrors |= valid;
            return valid;
        }

        public IEnumerable<IChange> ChangeTo(State other, ClientIdentity editor, DateTime time)
        {
            foreach (var (fileDescriptor, metadata) in _files)
            {
                if (!other._files.TryGetValue(fileDescriptor, out var otherMetadata))
                {
                    yield return new ChangeFileRemove(editor, time, fileDescriptor, metadata);
                }
                else if (otherMetadata != metadata)
                {
                    yield return new ChangeMetadata(editor, time, fileDescriptor, metadata, otherMetadata);
                }
            }

            foreach (var (fileDescriptor, metadata) in other._files)
            {
                if (!_files.ContainsKey(fileDescriptor))
                {
                    yield return new ChangeFileAdd(editor, time, fileDescriptor, metadata);
                }
            }
        }
    }
}
