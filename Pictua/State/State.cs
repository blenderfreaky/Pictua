using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace Pictua.StateTracking
{
    public class State
    {
        [JsonIgnore]
        public Dictionary<FileDescriptor, FileMetadata?> Metadata { get; set; } = new Dictionary<FileDescriptor, FileMetadata?>();

        [JsonPropertyName("Files"), Obsolete("Only for serialization")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213:Remove unused member declaration.", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private Dictionary<string, FileMetadata?> FilesJson
        {
            get => Metadata.ToDictionary(x => x.Key.UniqueName, x => x.Value);
            set => Metadata = value.ToDictionary(x => new FileDescriptor(Path.GetExtension(x.Key), x.Key), x => x.Value);
        }

        public FileMetadata? GetMetadata(FileDescriptor fileDescriptor) => Metadata.TryGetValue(fileDescriptor, out var metadata) ? metadata : null;

        public bool HasFile(FileDescriptor fileDescriptor) => Metadata.ContainsKey(fileDescriptor);

        public State(Dictionary<FileDescriptor, FileMetadata?> metadata)
        {
            Metadata = metadata;
        }

        public State()
        {
        }

        public bool Apply(IChange change)
        {
            var valid = change.Apply(this);
            return valid;
        }

        public bool Undo(IChange change)
        {
            var valid = change.Undo(this);
            return valid;
        }

        public IEnumerable<IChange> ChangeTo(State other, ClientIdentity editor, DateTime time)
        {
            foreach (var (fileDescriptor, metadata) in Metadata)
            {
                if (!other.Metadata.TryGetValue(fileDescriptor, out var otherMetadata))
                {
                    yield return new ChangeFileRemove(editor, time, fileDescriptor, metadata);
                }
                else if (otherMetadata != metadata)
                {
                    yield return new ChangeMetadata(editor, time, fileDescriptor, metadata, otherMetadata);
                }
            }

            foreach (var (fileDescriptor, metadata) in other.Metadata)
            {
                if (!Metadata.ContainsKey(fileDescriptor))
                {
                    yield return new ChangeFileAdd(editor, time, fileDescriptor, metadata);
                }
            }
        }

        public State Merge(State other)
        {
            return new State(Metadata.Union(other.Metadata).ToDictionary());
        }

        public State Clone() => new State(Metadata.ToDictionary());
    }
}