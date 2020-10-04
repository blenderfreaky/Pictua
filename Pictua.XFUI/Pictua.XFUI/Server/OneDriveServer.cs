using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pictua.OneDrive
{
    public class OneDriveServer : Server
    {
        [JsonIgnore]
        public OneDriveUser OneDriveUser { get; private set; }

        public OneDriveServer(OneDriveUser oneDriveUser, FilePathConfig filePaths, ILogger<OneDriveServer> logger) : base(filePaths, logger)
        {
            OneDriveUser = oneDriveUser;
        }

#nullable disable

        [Obsolete("Only for serialization")]
        private OneDriveServer() : base(null, null) { }

#nullable restore

        public static async Task<OneDriveServer> CreateAsync(OneDriveUser oneDriveUser, FilePathConfig filePaths, ILogger<OneDriveServer> logger)
        {
            try
            {
                var stateFileStream = await oneDriveUser.DownloadAsync(filePaths.StateFilePath).ConfigureAwait(false);
                var server = await JsonSerializer.DeserializeAsync<OneDriveServer>(stateFileStream).ConfigureAwait(false);
                server.FilePaths = filePaths;
                server.Logger = logger;
                server.OneDriveUser = oneDriveUser;
                return server;
            }
            catch (Exception) // TODO: Catch more precise exceptions
            {
                return new OneDriveServer(oneDriveUser, filePaths, logger);
            }
        }

        protected override async Task<bool> UploadAsync(Stream stream, string targetPath)
        {
            var driveItem = await OneDriveUser.UploadAsync(stream, targetPath).ConfigureAwait(false);
            return driveItem != null;
        }

        protected override async Task<bool> DownloadAsync(Stream stream, string originPath)
        {
            var downloadStream = await OneDriveUser.DownloadAsync(originPath).ConfigureAwait(false);
            await downloadStream.CopyToAsync(stream).ConfigureAwait(false);

            return true;
        }

        protected override Task<bool> DeleteAsync(string path)
        {
            return OneDriveUser.DeleteAsync(path);
        }

        protected override Task<bool> FileExistsAsnyc(string path)
        {
            return OneDriveUser.FileExistsAsync(path);
        }
    }
}