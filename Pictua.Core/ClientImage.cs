namespace Pictua.Core
{
    using System.Collections.Generic;

    public class ClientImage : IClientImage
    {
        public IClientIdentity Identity { get; }
        public long Storage { get; }
        public ICollection<IDiff> History { get; }
        public ICollection<FileDescriptor> StoredFiles { get; }

        public ClientImage(
            IClientIdentity identity,
            long storage,
            ICollection<IDiff> history,
            ICollection<FileDescriptor> storedFiles)
        {
            Identity = identity;
            Storage = storage;
            History = history;
            StoredFiles = storedFiles;
        }
    }


}
