using Microsoft.Graph;
using Microsoft.Identity.Client;
using Pictua.XFUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pictua.OneDrive
{
    public class OneDrive
    {
        private readonly App App;
        private GraphServiceClient _graphClient;

        public bool IsGraphClientInitialized => _graphClient != null;

        private IDriveItemRequestBuilder AppRoot => _graphClient.Drive.Special.AppRoot;

        public static string[] Scopes = { "Files.ReadWrite.AppFolder" };

        public OneDrive(App app)
        {
            App = app;
        }

        public async Task InitializeGraphClientAsync()
        {
            var currentAccounts = await App.PCA.GetAccountsAsync().ConfigureAwait(false);
            try
            {
                if (!currentAccounts.Any()) throw new Exception("No accounts found");

                // Initialize Graph client
                _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var result = await App.PCA.AcquireTokenSilent(Scopes, currentAccounts.FirstOrDefault())
                            .ExecuteAsync().ConfigureAwait(false);

                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialized graph client.");
                Debug.WriteLine($"Accounts in the msal cache: {currentAccounts.Count()}.");
                Debug.WriteLine($"See exception message for details: {ex.Message}");
            }
        }

        public async Task<bool> SignInAsync(bool forceSilent = false)
        {
            try
            {
                var accounts = await App.PCA.GetAccountsAsync().ConfigureAwait(false);
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await App.PCA.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync().ConfigureAwait(false);

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken).ConfigureAwait(false);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                if (forceSilent) return false;
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await App.PCA.AcquireTokenInteractive(Scopes)
                                                      //.WithUseEmbeddedWebView(true)
                                                      .WithParentActivityOrWindow(App.ParentWindow)
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
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.ToString());
            //    return false;
            //}
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await App.PCA.GetAccountsAsync().ConfigureAwait(false);

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await App.PCA.RemoveAsync(accounts.FirstOrDefault()).ConfigureAwait(false);
                    accounts = await App.PCA.GetAccountsAsync().ConfigureAwait(false);
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

        public static async Task<OneDrive> CreateAsync(App app)
        {
            var onedrive = new OneDrive(app);

            await onedrive.InitializeGraphClientAsync().ConfigureAwait(false);

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

        public Task<DriveItem> UploadAsync(Stream stream, string targetPath)
        {
            // TODO: Choose between small and large file
            return UploadLargeFileAsync(stream, targetPath);
        }

        public Task<DriveItem> UploadSmallFileAsync(Stream stream, string targetPath)
        {
            return AppRoot.ItemWithPath(targetPath).Content.Request()
                .PutAsync<DriveItem>(stream);
        }

        public async Task<DriveItem> UploadLargeFileAsync(Stream stream, string targetPath, int maxSliceSize = -1, IProgress<long> progress = null)
        {
            // TODO: Support fileUploadTask.ResumeAsnyc
            var uploadSession = await AppRoot.ItemWithPath(targetPath)
                            .CreateUploadSession().Request().PostAsync().ConfigureAwait(false);

            var fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, stream, maxSliceSize < 0 ? 1024 * 320 : maxSliceSize);

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
