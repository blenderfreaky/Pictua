using System;
using System.Collections;
using System.Collections.Generic;

namespace Pictua.HistoryTracking
{
    public class History : IEnumerable<IChange>
    {
        private readonly List<IChange> _diffs;

        public History(List<IChange> diffs)
        {
            _diffs = diffs;
            HeadState = new State(this);
        }

        public History() : this(new List<IChange>()) { }

        public History Clone() => new History(new List<IChange>(_diffs));

        public IChange this[int index] { get => _diffs[index]; }

        public int Count => _diffs.Count;

        public IChange Head => this[Count - 1];

        public State HeadState { get; }

        public bool Add(IChange change)
        {
            _diffs.Add(change);

            return HeadState.Apply(change);
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

        public History MergeFrom(History other)
        {
            var self = Clone();

            var commonOrigin = Math.Min(self.Count, other.Count) - 1;
            for (; commonOrigin >= 0; commonOrigin--)
            {
                if (self[commonOrigin] == other[commonOrigin]) break;
            }

            foreach (var otherChange in other.GetAllAfter(commonOrigin + 1))
            {
                self.Add(otherChange);
            }

            return self;
        }
    }
}
