using Microsoft.Extensions.Logging;
using Pictua.OneDrive;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pictua.XFUI
{
    public partial class MainPage : ContentPage
    {
        public Client Client;
        public OneDriveServer Server;

        private bool _signedIn;
        public bool SignedIn
        {
            get => _signedIn;
            set
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    SignInButton.IsVisible = !value;
                    SyncButton.IsVisible = value;
                });

                _signedIn = value;
            }
        }

        public MainPage()
        {
            var logger = LoggerFactory.Create(options =>
                options.AddConsole());

            Client = Client.Create(FilePathConfig.Client, logger.CreateLogger<Client>());

            Task.Run(async () => {
                Server = await OneDriveServer.Create((App)Application.Current, FilePathConfig.Server, logger.CreateLogger<OneDriveServer>()).ConfigureAwait(false);
                SignedIn = await Server.OneDrive.SignInAsync(true).ConfigureAwait(false);
            });

            InitializeComponent();
        }

        private void OnBack(object sender, EventArgs e)
        {
        }

        private async void SignInButton_Clicked(object sender, EventArgs e)
        {
            SignedIn = await Server.OneDrive.SignInAsync().ConfigureAwait(false);
        }

        private async void SyncButton_Clicked(object sender, EventArgs e)
        {
            await Client.SyncAsync(Server).ConfigureAwait(false);
        }
    }
}
