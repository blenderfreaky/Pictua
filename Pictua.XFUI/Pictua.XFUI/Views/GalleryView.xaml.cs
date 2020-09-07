using Pictua.XFUI.ViewModels;
using Plugin.FilePicker;
using ReactiveUI;
using ReactiveUI.XamForms;
using System.IO;
using Xamarin.Forms.Xaml;

namespace Pictua.XFUI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryView : ReactiveContentView<GalleryViewModel>
    {
        public GalleryView()
        {
            InitializeComponent();

            ViewModel = new GalleryViewModel();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.FlowListView.ItemsSource);
                this.Bind(ViewModel, vm => vm.LastTappedItem, v => v.FlowListView.FlowLastTappedItem);

                this.BindCommand(ViewModel, vm => vm.ItemTappedCommand, v => v.FlowListView.FlowItemTappedCommand);
                this.BindCommand(ViewModel, vm => vm.UploadFileCommand, v => v.UploadButton.Command);
            });
        }
    }
}