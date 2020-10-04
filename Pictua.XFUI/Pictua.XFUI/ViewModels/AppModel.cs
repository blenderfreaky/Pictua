using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Pictua.OneDrive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Buffers.Binary;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Pictua.XFUI.ViewModels
{
    public class AppModel : ReactiveObject
    {
        public Client Client { get; set; }
        private OneDriveUser OneDriveUser { get; set; }

        [Reactive]
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

            this.WhenAnyValue(x => x.IsSignedIn)
                .Where(x => x)
                .Subscribe(async _ =>
                    Server = await OneDriveServer.CreateAsync(OneDriveUser, FilePathConfig.Server, logger.CreateLogger<OneDriveServer>()).ConfigureAwait(false));

            Observable.Start(async () =>
            {
                OneDriveUser = OneDriveUser.Create(pca);

                await OneDriveUser.InitializeGraphClientAsync().ConfigureAwait(false);
                IsSignedIn = await OneDriveUser.SignInAsync(forceSilent: true).ConfigureAwait(false);
            });
        }
    }
}