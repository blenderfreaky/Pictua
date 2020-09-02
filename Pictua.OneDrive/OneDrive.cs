using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pictua.OneDrive
{
    public class OneDrive
    {
        private readonly IPublicClientApplication _app;
        private readonly GraphServiceClient _graphClient;

        private IDriveItemRequestBuilder AppRoot => _graphClient.Drive.Special.AppRoot;

        public OneDrive(IPublicClientApplication app, GraphServiceClient graphClient)
        {
            _app = app;
            _graphClient = graphClient;
        }

        private static readonly string[] Scopes = new[] { "Files.ReadWrite.AppFolder" };

        private static string AppId = "com.blenderfreaky.pictua";

        private static string RedirectUri
        {
            get
            {
                return "msalfd564916-e1a7-41aa-9e2e-5867cac60129://auth";

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    return $"msauth://{AppId}/{{ultNtp+zVc6mYSrp6uyg5zdU33A=}}";
                }
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    return $"msauth.{AppId}://auth";
                }
                else if (DeviceInfo.Platform == DevicePlatform.UWP)
                {
                    return "https://login.microsoftonline.com/common/oauth2/nativeclient";
                }

                return string.Empty;
            }
        }

        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await _app.GetAccountsAsync().ConfigureAwait(false);
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _app.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync().ConfigureAwait(false);

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken).ConfigureAwait(false);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _app.AcquireTokenInteractive(Scopes)
                                                .ExecuteAsync().ConfigureAwait(false);

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken).ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _app.GetAccountsAsync().ConfigureAwait(false);

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _app.RemoveAsync(accounts.FirstOrDefault()).ConfigureAwait(false);
                    accounts = await _app.GetAccountsAsync().ConfigureAwait(false);
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public static async Task<OneDrive> CreateAsync(string clientId)
        {
            var app = PublicClientApplicationBuilder
                               .Create(clientId)
                               .WithRedirectUri(RedirectUri)
                               .Build();

            var authProvider = new DeviceCodeProvider(app, Scopes);
            var graphClient = new GraphServiceClient(authProvider);

            var onedrive = new OneDrive(app, graphClient);

            await onedrive.SignInAsync().ConfigureAwait(false);

            return onedrive;
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
