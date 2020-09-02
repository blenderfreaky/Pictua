using Microsoft.Extensions.Logging;
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

        public static async Task<OneDriveServer> Create(string clientId, FilePathConfig filePaths, ILogger<OneDriveServer> logger)
        {
            return new OneDriveServer(await OneDrive.CreateAsync(clientId).ConfigureAwait(false), filePaths, logger);
        }

        public static Task<OneDriveServer> Create(FilePathConfig filePaths, ILogger<OneDriveServer> logger)
        {
            return Create("fd564916-e1a7-41aa-9e2e-5867cac60129", filePaths, logger);
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
