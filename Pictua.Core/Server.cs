namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class Server
    {
        public ICollection<IClientIdentity> Clients { get; }

        public IDictionary<FileDescriptor, ServerFile> Files { get; }

        public ICollection<MetaDataUpdate> NewMetadata { get; set; }

        public void ConfirmOwnership(FileDescriptor fileDescriptor, IClientIdentity clientIdentity)
        {
            if (Files.TryGetValue(fileDescriptor, out var file))
            {
                file.Owners.Add(clientIdentity);
            }
            else
            {
                Files[fileDescriptor] = new ServerFile(fileDescriptor, owners: new List<IClientIdentity> { clientIdentity });
            }
        }

        public Task ConfirmRemovalAsync(ServerFile serverFile, IClientIdentity clientIdentity)
        {
            serverFile.Owners.Remove(clientIdentity);

            if (serverFile.Owners.Count == 0)
            {
                if (serverFile.LocalPath == null) return;
                return DeleteLocalFileAsync(serverFile);
            }

            return Task.CompletedTask;
        }

        public void RequestRemoval(FileDescriptor fileDescriptor)
        {
            if (Files.TryGetValue(fileDescriptor, out var file))
            {
                file.RemovalRequested = true;
            }
            else
            {
                Files[fileDescriptor] = new ServerFile(fileDescriptor, removalRequested: true);
            }
        }

        public Task PushFileAsync(ConcreteFile concreteFile, IClientIdentity pusher)
        {
            var serverFile = new ServerFile(concreteFile.FileDescriptor, concreteFile.Metadata, null, false, new List<IClientIdentity> { pusher });
            Files[concreteFile.FileDescriptor] = serverFile;

            return PushFileAsync(concreteFile.LocalPath, serverFile);
        }

        protected abstract Task PushFileAsync(string origin, ServerFile target);

        public abstract Task<ConcreteFile> PullFileAsync(ServerFile file, IClientIdentity puller);

        public abstract Task LockAsync();
        public abstract Task UnlockAsync();

        public abstract Task DeleteLocalFileAsync(ServerFile file);

        public abstract Task CommitAsync();

        public Task CleanupAsync()
        {
            NewMetadata = NewMetadata.Where(x => !x.Owners.SequenceEqual(Clients)).ToList();
            return Task.WhenAll(Files.Values.Where(x => x.Owners.SequenceEqual(Clients)).Select(DeleteLocalFileAsync));
        }
    }
}
