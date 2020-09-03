using Microsoft.Extensions.Logging;
using Pictua.OneDrive;
using Pictua.XFUI.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pictua.XFUI
{
    public partial class MainPage : ReactiveContentPage<MainViewModel>
    {
        public MainPage()
        {
            //    App.Current.ViewModel
            //        .WhenAnyValue(x => x.IsSignedIn)
            //        .Throttle(TimeSpan.FromMilliseconds(200))
            //        .Subscribe(value =>
            //    {
            //        SignInButton.IsVisible = !value;
            //        SyncButton.IsVisible = value;
            //    });

            InitializeComponent();

            ViewModel = new MainViewModel();

            this.OneWayBind(App.Current.ViewModel, vm => vm.IsSignedIn, v => v.SignInButton);
            this.OneWayBind(App.Current.ViewModel, vm => vm.IsSignedIn, v => v.SyncButton);
        }

        private void OnBack(object sender, EventArgs e)
        {
        }

        private async void SignInButton_Clicked(object sender, EventArgs e)
        {
            var app = App.Current.ViewModel;
            app.IsSignedIn = await app.Server.OneDrive.SignInAsync().ConfigureAwait(false);
            if (app.IsSignedIn && !app.Server.OneDrive.IsGraphClientInitialized) await app.Server.OneDrive.InitializeGraphClientAsync().ConfigureAwait(false);
        }

        private async void SyncButton_Clicked(object sender, EventArgs e)
        {
            var app = App.Current.ViewModel;
            await app.Client.SyncAsync(app.Server).ConfigureAwait(false);
        }
    }
}
