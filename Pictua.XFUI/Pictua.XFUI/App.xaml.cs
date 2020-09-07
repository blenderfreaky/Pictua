using DLToolkit.Forms.Controls;
using Pictua.XFUI.ViewModels;
using Xamarin.Forms;

namespace Pictua.XFUI
{
    public partial class App : Application
    {
        public static new App Current => (App)Application.Current;

        public static string MsalClientID = "fd564916-e1a7-41aa-9e2e-5867cac60129";
        public static string[] MsalScopes = { "Files.ReadWrite.AppFolder" };

        public static object AuthParentWindow { get; set; }

        public AppModel ViewModel { get; }

        public App(string specialRedirectUri = null)
        {
            FlowListView.Init();

            InitializeComponent();

            ViewModel = new AppModel(specialRedirectUri);

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}