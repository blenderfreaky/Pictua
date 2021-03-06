﻿using Microsoft.Extensions.Logging;
using Pictua.StateTracking;
using Pictua.Tags;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pictua
{
    public class Client : IDisposable
    {
        [JsonIgnore]
        public FilePathConfig FilePaths { get; private set; }

        public ClientIdentity Identity { get; }

        public State State { get; set; }

        public ISet<FileDescriptor> FilesAwaitingDownload { get; }
        public ISet<FileDescriptor> FilesAwaitingUpload { get; }

        [JsonIgnore]
        public IDictionary<FileDescriptor, DateTime> DeletedOn { get; private set; }

        [JsonPropertyName("DeletedOn"), Obsolete("Only for serialization")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213:Remove unused member declaration.", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private Dictionary<string, string> DeletedOnJson
        {
            get => DeletedOn.ToDictionary(x => x.Key.UniqueName, x => x.Value.ToString());
            set => DeletedOn = value.ToDictionary(x => new FileDescriptor(Path.GetExtension(x.Key), x.Key), x => DateTime.Parse(x.Value));
        }

        [JsonIgnore]
        protected ILogger<Client> Logger { get; set; }

        [JsonIgnore]
        private FileStream? LockFileStream { get; set; }

        private bool disposedValue;

#nullable disable

        [Obsolete("Only for serialization")]
        public Client() { }

#nullable restore

        protected Client(FilePathConfig filePaths, ClientIdentity identity, ILogger<Client> logger)
        {
            FilePaths = filePaths;
            Identity = identity;
            State = new State();
            FilesAwaitingDownload = new HashSet<FileDescriptor>();
            FilesAwaitingUpload = new HashSet<FileDescriptor>();
            DeletedOn = new Dictionary<FileDescriptor, DateTime>();
            Logger = logger;
        }

        public static Client Create(FilePathConfig clientFiles, ILogger<Client> logger)
        {
            if (!File.Exists(clientFiles.StateFilePath))
            {
                return CreateNew(clientFiles, logger);
            }

            try
            {
                var client = JsonSerializer.Deserialize<Client>(File.ReadAllText(clientFiles.StateFilePath));
                client.FilePaths = clientFiles;
                client.Logger = logger;
                return client;
            }
            catch (FileNotFoundException)
            {
                return CreateNew(clientFiles, logger);
            }
            catch (DirectoryNotFoundException)
            {
                return CreateNew(clientFiles, logger);
            }
        }

        private static Client CreateNew(FilePathConfig clientFiles, ILogger<Client> logger)
        {
            return new Client(clientFiles, new ClientIdentity(Environment.MachineName), logger);
        }

        public void Commit()
        {
            System.IO.File.WriteAllBytes(FilePaths.StateFilePath, JsonSerializer.SerializeToUtf8Bytes(this, GetType()));
        }

        public bool Lock()
        {
            try
            {
                if (System.IO.File.Exists(FilePaths.LockFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(FilePaths.LockFilePath);
                    }
                    catch (IOException)
                    {
                        return false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return false;
                    }
                }

                var directory = Path.GetDirectoryName(FilePaths.LockFilePath);

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                LockFileStream = new FileStream(FilePaths.LockFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public void Unlock()
        {
            LockFileStream?.Dispose();
            System.IO.File.Delete(FilePaths.LockFilePath);
        }

        public async Task SyncAsync(Server server)
        {
            if (!Lock()) throw new Exception("Client locking failed.");
            if (!await server.LockAsync().ConfigureAwait(false)) throw new Exception("Server locking failed.");

            var merged = State.Merge(server.State);

            var localChanges = State.ChangeTo(merged, Identity, DateTime.UtcNow).Select(x => ApplyChangeLocally(x, server));
            var serverChanges = server.State.ChangeTo(merged, Identity, DateTime.UtcNow).Select(x => ApplyChangeOnServer(x, server));

            await Task.WhenAll(localChanges.Concat(serverChanges)).ConfigureAwait(false);

            State = merged;
            server.State = merged.Clone();

            Commit();
            await server.CommitAsync().ConfigureAwait(false);

            Unlock();
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
                        System.IO.File.Move(FilePaths.GetFilePath(fileRemove.File), FilePaths.GetTrashFilePath(fileRemove.File));
                        DeletedOn[fileRemove.File] = DateTime.UtcNow;
                    }
                    catch (IOException ex)
                    {
                        Logger.LogError(ex, $"Error deleting file: {fileRemove.File}");
                    }
                    break;

                case ChangeMetadata metadata:
                    State.Metadata[metadata.Target] = metadata.NewMetadata;
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

        public FileDescriptor? AddFile(string fileExtension, string title, Stream stream, bool overwrite = false)
        {
            var descriptor = new FileDescriptor(fileExtension, FileHashes.CalculateMD5(stream));
            var doesFileExistsAlready = State.Metadata.ContainsKey(descriptor);

            if (doesFileExistsAlready && !overwrite) return null;

            stream.Seek(0, SeekOrigin.Begin);

            var path = FilePaths.GetFilePath(descriptor);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using var fileStream = System.IO.File.OpenWrite(path);
            stream.CopyTo(fileStream);

            Debug.WriteLine($"Copying {title} to {path}");

            if (!doesFileExistsAlready)
            {
                lock (State.Metadata)
                {
                    State.Metadata[descriptor] = new FileMetadata(DateTime.UtcNow, new List<ITag> { new StringTag("Title", title) });
                }
            }

            return descriptor;
        }

        public Stream GetFile(FileDescriptor fileDescriptor)
        {
            var path = FilePaths.GetFilePath(fileDescriptor);

            return System.IO.File.OpenRead(path);
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

        #endregion IDisposable
    }
}