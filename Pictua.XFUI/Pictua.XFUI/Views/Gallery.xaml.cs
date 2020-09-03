using Pictua.XFUI.ViewModels;
using Plugin.FilePicker;
using ReactiveUI.XamForms;
using System.IO;
using Xamarin.Forms.Xaml;

namespace Pictua.XFUI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gallery : ReactiveContentView<GalleryViewModel>
    {
        public Gallery()
        {
            InitializeComponent();
        }

        private async void UploadButton_Clicked(object sender, System.EventArgs e)
        {
            var file = await CrossFilePicker.Current.PickFile().ConfigureAwait(false);
            App.Current.ViewModel.Client.AddFile(Path.GetExtension(file.FileName), file.GetStream());
        }
    }
}