using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pictua.HistoryTracking
{
    public class State
    {
        public ICollection<FileDescriptor> Files { get; }
        public IDictionary<FileDescriptor, FileMetadata> Metadata { get; }

        public State(ICollection<FileDescriptor> files, IDictionary<FileDescriptor, FileMetadata> metadata)
        {
            Files = files;
            Metadata = metadata;
        }

        public State() : this(new List<FileDescriptor>(), new Dictionary<FileDescriptor, FileMetadata>()) { }

        public State(History history) : this()
        {
            //foreach (var )
        }
    }

    public class History : IEnumerable<IChange>
    {
        private readonly List<IChange> _diffs;

        public History(List<IChange> diffs)
        {
            _diffs = diffs;
            HeadState = new State(this);
        }

        public History() : this(new List<IChange>()) { }

        public IChange this[int index] { get => _diffs[index]; }

        public int Count => _diffs.Count;

        public IChange Head => this[Count - 1];

        public State HeadState { get; }

        public void Add(IChange item)
        {
            _diffs.Add(item);
        }

        public IEnumerator<IChange> GetEnumerator() => _diffs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _diffs.GetEnumerator();

        public IEnumerable<IChange> Reverse()
        {
            for (int i = Count - 1; i >= 0; i++)
            {
                yield return this[i];
            }
        }

        public IEnumerable<IChange> GetAllAfter(DateTime dateTime)
        {
            int start = 0;
            for (int i = Count - 1; i >= 0; i++)
            {
                if (this[i].Time > dateTime) continue;
                start = i;
                break;
            }

            return GetAllAfter(start);
        }

        public IEnumerable<IChange> GetAllAfter(int index)
        {
            for (int i = index; i < Count; i++)
            {
                yield return this[i];
            }
        }

        public IEnumerable<IChange> MergeFrom(History other)
        {
            var commonOrigin = Math.Min(Count, other.Count);
            for (; commonOrigin >= 0; commonOrigin--)
            {
                if (this[commonOrigin] == other[commonOrigin]) break;
            }

            foreach (var otherChange in other.GetAllAfter(commonOrigin + 1))
            {
                Add(otherChange);
                yield return otherChange;
            }
        }
    }
}
