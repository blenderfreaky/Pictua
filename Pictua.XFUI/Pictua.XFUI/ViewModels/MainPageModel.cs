using ReactiveUI;
using System.Windows.Input;

namespace Pictua.XFUI.ViewModels
{
    public class MainPageModel : ReactiveObject
    {
        public MainPageModel()
        {
            SignIn = ReactiveCommand.CreateFromTask(async ()=>
            {
                var app = App.Current.ViewModel;
                IsSignedIn = await app.Server.OneDrive.SignInAsync().ConfigureAwait(false);
                if (IsSignedIn && !app.Server.OneDrive.IsGraphClientInitialized) await app.Server.OneDrive.InitializeGraphClientAsync().ConfigureAwait(false);
            });

            SyncIn = ReactiveCommand.CreateFromTask(async () =>
            {
                var app = App.Current.ViewModel;
                await app.Client.SyncAsync(app.Server).ConfigureAwait(false);
            });
        }

        public bool IsSignedIn { get => App.Current.ViewModel.IsSignedIn; set => App.Current.ViewModel.IsSignedIn = value; }

        public ICommand SignIn { get; }
        public ICommand SyncIn { get; }
    }
}