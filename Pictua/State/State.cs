using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Pictua.StateTracking
{
    public class State
    {
        [XmlIgnore]
        public Dictionary<FileDescriptor, FileMetadata?> Metadata = new Dictionary<FileDescriptor, FileMetadata?>();

        public List<File> Files
        {
            get { return Metadata.Select(x => new File(x.Key, x.Value)).ToList(); }
            set { Metadata = value.ToDictionary(x => x.Descriptor, x => x.Metadata); }
        }

        public FileMetadata? GetMetadata(FileDescriptor fileDescriptor) => Metadata.TryGetValue(fileDescriptor, out var metadata) ? metadata : null;

        public bool HasFile(FileDescriptor fileDescriptor) => Metadata.ContainsKey(fileDescriptor);

        public State(Dictionary<FileDescriptor, FileMetadata?> files)
        {
            Metadata = files;
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

    public struct File
    {
        public FileDescriptor Descriptor { get; set; }
        public FileMetadata? Metadata { get; set; }

        public File(FileDescriptor descriptor, FileMetadata? metadata)
        {
            Descriptor = descriptor;
            Metadata = metadata;
        }

        public override bool Equals(object? obj) => obj is File other &&
                   Descriptor.Equals(other.Descriptor) &&
                   EqualityComparer<FileMetadata?>.Default.Equals(Metadata, other.Metadata);

        public override int GetHashCode() => HashCode.Combine(Descriptor, Metadata);
    }
}