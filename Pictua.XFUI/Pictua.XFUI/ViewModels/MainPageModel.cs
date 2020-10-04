using ReactiveUI;
using System.Windows.Input;

namespace Pictua.XFUI.ViewModels
{
    public class MainPageModel : ReactiveObject
    {
        public MainPageModel()
        {
            SignIn = ReactiveCommand.CreateFromTask(async () =>
            {
                var app = App.Current.ViewModel;
                app.IsSignedIn = await app.Server.OneDriveUser.SignInAsync().ConfigureAwait(false);
                if (app.IsSignedIn && !app.Server.OneDriveUser.IsGraphClientInitialized) await app.Server.OneDriveUser.InitializeGraphClientAsync().ConfigureAwait(false);
            });

            SyncIn = ReactiveCommand.CreateFromTask(async () =>
            {
                var app = App.Current.ViewModel;
                await app.Client.SyncAsync(app.Server).ConfigureAwait(false);
            });
        }

        public ICommand SignIn { get; }
        public ICommand SyncIn { get; }
    }
}