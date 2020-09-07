using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Pictua.OneDrive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;

namespace Pictua.XFUI.ViewModels
{
    public class AppModel : ReactiveObject
    {
        public Client Client { get; set; }
        public OneDriveServer Server { get; set; }

        [Reactive]
        public bool IsSignedIn { get; set; }

        public AppModel(string specialRedirectUri = null)
        {
            var pca = PublicClientApplicationBuilder.Create(App.MsalClientID)
                .WithRedirectUri(specialRedirectUri ?? $"msal{App.MsalClientID}://auth")
                .WithIosKeychainSecurityGroup("com.blenderfreaky.Pictua")
                .WithParentActivityOrWindow(() => App.AuthParentWindow)
                .Build();

            var logger = LoggerFactory.Create(options =>
                options.AddConsole());

            Client = Client.Create(FilePathConfig.Client, logger.CreateLogger<Client>());
            Server = OneDriveServer.Create(pca, FilePathConfig.Server, logger.CreateLogger<OneDriveServer>());

            Task.Run(async () =>
            {
                await Server.OneDrive.InitializeGraphClientAsync().ConfigureAwait(false);
                IsSignedIn = await Server.OneDrive.SignInAsync(forceSilent: true).ConfigureAwait(false);
            });
        }
    }
}