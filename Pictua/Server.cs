using Microsoft.Extensions.Logging;
using Pictua.HistoryTracking;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pictua
{
    public abstract class Server
    {
        [XmlIgnore]
        public FilePathConfig FilePaths { get; }

        public ISet<ClientIdentity> Clients { get; }

        public IDictionary<FileDescriptor, ServerFileInfo> Files { get; }

        public History History { get; protected internal set; }

        [XmlIgnore]
        protected ILogger<Server> Logger { get; set; }

        public ServerFileInfo this[FileDescriptor fileDescriptor]
        {
            get
            {
                if (Files.TryGetValue(fileDescriptor, out var val)) return val;
                return Files[fileDescriptor] = new ServerFileInfo();
            }
        }

        protected Server(FilePathConfig filePaths, ILogger<Server> logger)
        {
            FilePaths = filePaths;
            Clients = new HashSet<ClientIdentity>();
            Files = new Dictionary<FileDescriptor, ServerFileInfo>();
            History = new History();
            Logger = logger;
        }

        public async void ConfirmDownloadAsync(FileDescriptor fileDescriptor, ClientIdentity clientIdentity)
        {
            var serverFileInfo = Files[fileDescriptor];
            var owners = serverFileInfo.Owners;
            owners.Add(clientIdentity);

            if (owners.SetEquals(Clients))
            {
                if (await DeleteFileAsync(fileDescriptor).ConfigureAwait(false))
                {
                    serverFileInfo.IsContentOnline = false;
                }
                else
                {
                    Logger.LogError($"File Deletion failed: {fileDescriptor}, deleted because {clientIdentity} downloaded the file, serving it to all clients.");
                }
            }
        }

        protected abstract Task<bool> FileExistsAsnyc(string path);
        protected abstract Task<bool> UploadAsync(Stream stream, string targetPath);
        protected abstract Task<bool> DownloadAsync(Stream stream, string originPath);
        protected abstract Task<bool> DeleteAsync(string path);

        public virtual Task<bool> UploadAsync(FileDescriptor fileDescriptor, string originPath)
        {
            using var fileStream = File.OpenRead(originPath);
            return UploadAsync(fileStream, FilePaths.GetFilePath(fileDescriptor));
        }

        public virtual Task<bool> DownloadAsync(FileDescriptor fileDescriptor, string targetPath)
        {
            using var fileStream = File.OpenWrite(targetPath);
            return DownloadAsync(fileStream, FilePaths.GetFilePath(fileDescriptor));
        }

        public virtual Task<bool> DeleteFileAsync(FileDescriptor file)
        {
            return DeleteAsync(FilePaths.GetFilePath(file));
        }

        public virtual async Task<bool> LockAsync()
        {
            // TODO: Fix race condition
            if (await FileExistsAsnyc(FilePaths.LockFilePath).ConfigureAwait(false)) return false;
            return await UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("Lock")), FilePaths.LockFilePath).ConfigureAwait(false);
        }

        public virtual Task<bool> UnlockAsync()
        {
            return DeleteAsync(FilePaths.LockFilePath);
        }

        public virtual Task<bool> CommitAsync()
        {
            using var memStream = new MemoryStream();
            Xml.Serialize(GetType(), memStream, this);
            return UploadAsync(memStream, FilePaths.StateFilePath);
        }
    }
}
