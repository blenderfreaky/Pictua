using Microsoft.Extensions.Logging;
using Pictua.HistoryTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pictua
{
    public class Client : IDisposable
    {
        [XmlIgnore]
        public FilePathConfig FilePaths { get; private set; }

        public ClientIdentity Identity { get; }

        public History History { get; protected internal set; }

        public IDictionary<FileDescriptor, FileMetadata?> Files { get; }

        public ISet<FileDescriptor> FilesAwaitingDownload { get; }
        public ISet<FileDescriptor> FilesAwaitingUpload { get; }

        public IDictionary<FileDescriptor, DateTime> DeletedOn { get; }

        [XmlIgnore]
        protected ILogger<Client> Logger { get; set; }

        [XmlIgnore]
        private FileStream? LockFileStream { get; set; }

        [XmlIgnore]
        private bool disposedValue;

        protected Client(FilePathConfig filePaths, ClientIdentity identity, ILogger<Client> logger)
        {
            FilePaths = filePaths;
            Identity = identity;
            History = new History();
            Files = new Dictionary<FileDescriptor, FileMetadata?>();
            FilesAwaitingDownload = new HashSet<FileDescriptor>();
            FilesAwaitingUpload = new HashSet<FileDescriptor>();
            DeletedOn = new Dictionary<FileDescriptor, DateTime>();
            Logger = logger;
        }

        public static Client Create(FilePathConfig clientFiles, ILogger<Client> logger)
        {
            try
            {
                using var fileStream = File.OpenRead(clientFiles.StateFilePath);
                var client = Xml.Deserialize<Client>(fileStream);
                client.FilePaths = clientFiles;
                client.Logger = logger;
                return client;
            }
            catch (FileNotFoundException)
            {
                return new Client(clientFiles, new ClientIdentity(Environment.MachineName), logger);
            }
        }

        public void Commit()
        {
            using var fileStream = File.OpenWrite(FilePaths.StateFilePath);
            Xml.Serialize(GetType(), fileStream, this);
        }

        public bool Lock()
        {
            try
            {
                LockFileStream = new FileStream(FilePaths.LockFilePath, FileMode.CreateNew, FileAccess.Read, FileShare.None);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public void Unlock()
        {
            LockFileStream?.Dispose();
            File.Delete(FilePaths.LockFilePath);
        }

        public async Task SyncAsync(Server server)
        {
            await server.LockAsync().ConfigureAwait(false);

            var merged = server.History.MergeFrom(History);

            var localChanges = History.HeadState.ChangeTo(merged.HeadState, Identity, DateTime.UtcNow).Select(x => ApplyChangeLocally(x, server));
            var serverChanges = server.History.HeadState.ChangeTo(merged.HeadState, Identity, DateTime.UtcNow).Select(x => ApplyChangeOnServer(x, server));

            await Task.WhenAll(localChanges.Concat(serverChanges)).ConfigureAwait(false);

            History = merged;
            server.History = merged.Clone();

            await server.CommitAsync().ConfigureAwait(false);

            await server.UnlockAsync().ConfigureAwait(false);
        }

        public async Task ApplyChangeLocally(IChange change, Server server)
        {
            switch (change)
            {
                case ChangeFileAdd fileAdd:
                    if (await server.DownloadAsync(fileAdd.File, FilePaths.GetFilePath(fileAdd.File)).ConfigureAwait(false))
                    {
                        server.Files[fileAdd.File].Owners.Add(Identity);
                    }
                    else
                    {
                        FilesAwaitingDownload.Add(fileAdd.File);
                        Logger.LogError($"Error downloading file: {fileAdd.File}");
                    }
                    break;

                case ChangeFileRemove fileRemove:
                    try
                    {
                        File.Move(FilePaths.GetFilePath(fileRemove.File), FilePaths.GetTrashFilePath(fileRemove.File));
                        DeletedOn[fileRemove.File] = DateTime.UtcNow;
                    }
                    catch (IOException ex)
                    {
                        Logger.LogError(ex, $"Error deleting file: {fileRemove.File}");
                    }
                    break;

                case ChangeMetadata metadata:
                    Files[metadata.Target] = metadata.NewMetadata;
                    break;

                default:
                    throw new ArgumentException("Unknown change type", nameof(change));
            }
        }

        public async Task ApplyChangeOnServer(IChange change, Server server)
        {
            switch (change)
            {
                case ChangeFileAdd fileAdd:
                    if (!await server.UploadAsync(fileAdd.File, FilePaths.GetFilePath(fileAdd.File)).ConfigureAwait(false))
                    {
                        FilesAwaitingUpload.Add(fileAdd.File);
                        Logger.LogError($"Error uploading file: {fileAdd.File}");
                    }
                    break;

                case ChangeFileRemove fileRemove:
                    if (!await server.DeleteFileAsync(fileRemove.File).ConfigureAwait(false))
                    {
                        Logger.LogError($"Error deleting server file: {fileRemove.File}");
                    }
                    break;

                case ChangeMetadata metadata:
                    server[metadata.Target].Metadata = metadata.NewMetadata;
                    break;

                default:
                    throw new ArgumentException("Unknown change type", nameof(change));
            }
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                Unlock();
                disposedValue = true;
            }
        }
        ~Client()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}