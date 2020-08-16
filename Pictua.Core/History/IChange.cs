using Pictua.HistoryTracking;
using System;

namespace Pictua
{
    public interface IChange
    {
        ClientIdentity Author { get; }

        DateTime Time { get; }

        bool Apply(State state);
        bool Undo(State state);
    }
}
