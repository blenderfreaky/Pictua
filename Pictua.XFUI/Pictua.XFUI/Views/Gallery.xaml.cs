
using Plugin.FilePicker;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pictua.XFUI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gallery : ContentView
    {
        public Gallery()
        {
            InitializeComponent();
        }

        private async void UploadButton_Clicked(object sender, System.EventArgs e)
        {
            var mainPage = FindMainPage(Parent);
            var file = await CrossFilePicker.Current.PickFile().ConfigureAwait(false);
            mainPage.Client.AddFile(Path.GetExtension(file.FileName), file.GetStream());
        }

        private static MainPage FindMainPage(Element element) => element is MainPage mainPage ? mainPage : element == null ? null : FindMainPage(element.Parent);
    }
}