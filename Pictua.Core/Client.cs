namespace Pictua.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Client
    {
        public IClientIdentity Identity { get; }

        public IDictionary<FileDescriptor, ConcreteFile> StoredFiles { get; }

        public async Task SyncAsync(Server server)
        {
            await server.LockAsync().ConfigureAwait(false);

            var push = StoredFiles
                .Where(x => !server.Files.TryGetValue(x.FileDescriptor, out var file) || file.LocalPath == null)
                .Select(x => server.PushFileAsync(x, Identity));

            var pull = server.Files.Values
                .Where(x => !x.RemovalRequested && StoredFiles.All(y => y.FileDescriptor != x.FileDescriptor))
                .Select(serverFile => PullFileAsync(serverFile, server));

            var delete = server.Files.Values
                .Where(x => x.RemovalRequested)
                .Select(x => RemoveFileAsync(x, server));

            await Task.WhenAll(push.Concat(pull).Concat(delete)).ConfigureAwait(false);

            foreach (var serverMetadata in server.NewMetadata)
            {
                if (StoredFiles.TryGetValue(serverMetadata.FileDescriptor, out var local)
                    && local.Metadata.LastUpdated < serverMetadata.Metadata.LastUpdated)
                {
                    local.Metadata = serverMetadata.Metadata;
                    serverMetadata.Owners.Add(Identity);
                }
            }

            foreach (var localFile in StoredFiles.Values)
            {
                if (!server.Files.TryGetValue(localFile.FileDescriptor, out var file)) continue;
                if (file.Metadata == null) continue;

                if (localFile.Metadata.LastUpdated <= file.Metadata.Value.LastUpdated)
                {
                    // TODO
                    //localFile.Metadata = file.Metadata.Value;
                    continue;
                }

                server.NewMetadata.Add(
                    new MetaDataUpdate(localFile.FileDescriptor, localFile.Metadata, new List<IClientIdentity> { Identity }));
            }

            await server.CleanupAsync().ConfigureAwait(false);

            await server.CommitAsync().ConfigureAwait(false);

            await server.UnlockAsync().ConfigureAwait(false);
        }

        protected async Task PullFileAsync(ServerFile serverFile, Server server)
        {
            var file = await server.PullFileAsync(serverFile, Identity).ConfigureAwait(false);

            if (serverFile.FileDescriptor != file.FileDescriptor) throw new InvalidOperationException("Invalid download");

            StoredFiles[file.FileDescriptor] = file;

            server.ConfirmOwnership(file.FileDescriptor, Identity);
        }

        protected Task RemoveFileAsync(ServerFile serverFile, Server server)
        {
            if (!StoredFiles.TryGetValue(serverFile.FileDescriptor, out var local))
            {
                return server.ConfirmRemovalAsync(serverFile, Identity);
            }

            File.Delete(local.LocalPath);

            return server.ConfirmRemovalAsync(serverFile, Identity);
        }
    }
}
