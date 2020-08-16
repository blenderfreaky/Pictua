using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pictua.OneDrive
{
    public class OneDrive
    {
        private readonly IPublicClientApplication _app;
        private readonly IAuthenticationProvider _authProvider;
        private readonly GraphServiceClient _graphClient;

        private IDriveItemRequestBuilder AppRoot => _graphClient.Drive.Special.AppRoot;

        public OneDrive(string clientId)
        {
            _app = PublicClientApplicationBuilder
                               .Create(clientId)
                               .Build();

            _authProvider = new InteractiveAuthenticationProvider(_app, new[] { "Files.ReadWrite.AppFolder" });

            _graphClient = new GraphServiceClient(_authProvider);
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);
            var children = await AppRoot.ItemWithPath(directory).Children.Request().GetAsync().ConfigureAwait(false);
            return children.Any(x => x.Name == name);
        }

        public async Task<bool> DeleteAsync(string path)
        {
            await AppRoot.ItemWithPath(path).Request().DeleteAsync().ConfigureAwait(false);
            return true;
        }

        public Task<Stream> DownloadAsync(string originPath)
        {
            return AppRoot.ItemWithPath(originPath).Content.Request().GetAsync();
        }

        public Task<DriveItem?> UploadAsync(Stream stream, string targetPath)
        {
            // TODO: Choose between small and large file
            return UploadLargeFileAsync(stream, targetPath);
        }

        public Task<DriveItem> UploadSmallFileAsync(Stream stream, string targetPath)
        {
            return AppRoot.ItemWithPath(targetPath).Content.Request()
                .PutAsync<DriveItem>(stream);
        }

        public async Task<DriveItem?> UploadLargeFileAsync(Stream stream, string targetPath, int maxSliceSize = -1, IProgress<long>? progress = null)
        {
            // TODO: Support fileUploadTask.ResumeAsnyc
            var uploadSession = await AppRoot.ItemWithPath(targetPath)
                            .CreateUploadSession().Request().PostAsync().ConfigureAwait(false);

            var fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, stream, maxSliceSize);

            try
            {
                var uploadResult = await fileUploadTask.UploadAsync(progress).ConfigureAwait(false);

                if (uploadResult.UploadSucceeded)
                {
                    return uploadResult.ItemResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (ServiceException)
            {
                throw;
            }
        }
    }
}
