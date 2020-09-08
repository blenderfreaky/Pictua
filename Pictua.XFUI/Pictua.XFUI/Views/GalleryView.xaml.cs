using Pictua.XFUI.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
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
                d(this.OneWayBind(ViewModel, vm => vm.Items, v => v.FlowListView.FlowItemsSource));
                d(this.Bind(ViewModel, vm => vm.LastTappedItem, v => v.FlowListView.FlowLastTappedItem));

                d(this.BindCommand(ViewModel, vm => vm.ItemTappedCommand, v => v.FlowListView.FlowItemTappedCommand));

                d(this.BindCommand(ViewModel, vm => vm.UploadFileCommand, v => v.UploadFileButton));
                d(this.BindCommand(ViewModel, vm => vm.UploadPhotoCommand, v => v.UploadPhotosButton));
                d(this.BindCommand(ViewModel, vm => vm.UploadPhotosCommand, v => v.UploadPhotosButton));
                d(this.BindCommand(ViewModel, vm => vm.UploadVideoCommand, v => v.UploadVideoButton));
            });
        }
    }
}