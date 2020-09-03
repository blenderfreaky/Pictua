using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pictua.OneDrive
{
    public class OneDriveServer : Server
    {
        public OneDrive OneDrive { get; }

        public OneDriveServer(OneDrive oneDrive, FilePathConfig filePaths, ILogger<OneDriveServer> logger) : base(filePaths, logger)
        {
            OneDrive = oneDrive;
        }

#nullable disable

        [Obsolete("Only for serialization")]
        private OneDriveServer() : base(null, null) { }

#nullable restore

        public static OneDriveServer Create(IPublicClientApplication pca, FilePathConfig filePaths, ILogger<OneDriveServer> logger)
        {
            return new OneDriveServer(OneDrive.Create(pca), filePaths, logger);
        }

        protected override async Task<bool> UploadAsync(Stream stream, string targetPath)
        {
            var driveItem = await OneDrive.UploadAsync(stream, targetPath).ConfigureAwait(false);
            return driveItem != null;
        }

        protected override async Task<bool> DownloadAsync(Stream stream, string originPath)
        {
            var downloadStream = await OneDrive.DownloadAsync(originPath).ConfigureAwait(false);
            await downloadStream.CopyToAsync(stream).ConfigureAwait(false);

            return true;
        }

        protected override Task<bool> DeleteAsync(string path)
        {
            return OneDrive.DeleteAsync(path);
        }

        protected override Task<bool> FileExistsAsnyc(string path)
        {
            return OneDrive.FileExistsAsync(path);
        }
    }
}