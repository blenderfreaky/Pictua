using System;
using System.Collections.Generic;

namespace Pictua.Core
{
    public interface IDevice
    {
        string Name { get; }
        long Storage { get; }

        ICollection<ISyncState> SyncDates { get; }

        ICollection<IFileDescriptor> StoredFiles { get; }
    }

    public interface ICloudStorage
    {
        string Name { get; }
        long Storage { get; }

        ICollection<ISyncState> SyncDates { get; }

        IDictionary<IFileDescriptor, ICollection<IDevice>> FileOwners { get; }
    }

    public interface ISyncState
    {
        IDevice Device { get; }
        ICloudStorage CloudStorage { get; }

        DateTime Time { get; }
        IError? Error { get; }

        ICollection<IFileDescriptor> NewOnCloud { get; }
        ICollection<IFileDescriptor> NewOnClient { get; }
    }

    public interface IError
    {
        string ShortName { get; }
    }
}
