using Pictua.XFUI.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Pictua.XFUI
{
    public partial class MainPage : ReactiveContentPage<MainPageModel>
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainPageModel();

            this.WhenActivated(d =>
            {
                d(this.OneWayBind(App.Current.ViewModel, vm => vm.IsSignedIn, v => v.SignInButton.IsVisible, isSignedIn => !isSignedIn));
                d(this.OneWayBind(App.Current.ViewModel, vm => vm.IsSignedIn, v => v.SyncButton.IsVisible, isSignedIn => isSignedIn));

                d(this.BindCommand(ViewModel, vm => vm.SignIn, v => v.SignInButton));
                d(this.BindCommand(ViewModel, vm => vm.SyncIn, v => v.SyncButton));
            });
        }
    }
}
