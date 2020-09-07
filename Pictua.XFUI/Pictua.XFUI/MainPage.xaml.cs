using Pictua.XFUI.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.XamForms;
using System;
using System.Reactive.Linq;

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
                this.OneWayBind(ViewModel, vm => vm.IsSignedIn, v => v.SignInButton.IsVisible, isSignedIn => !isSignedIn);
                this.OneWayBind(ViewModel, vm => vm.IsSignedIn, v => v.SyncButton.IsVisible, isSignedIn => isSignedIn);
            });
        }
    }
}
