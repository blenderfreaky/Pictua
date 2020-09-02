using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace Pictua.XFUI
{
    public partial class App : Application
    {
        public IPublicClientApplication PCA;

        public static string ClientID = "fd564916-e1a7-41aa-9e2e-5867cac60129";

        public static string[] Scopes = { "Files.ReadWrite.AppFolder" };
        public static string Username = string.Empty;

        public static object ParentWindow { get; set; }

        public App(string specialRedirectUri = null)
        {
            InitializeComponent();

            PCA = PublicClientApplicationBuilder.Create(ClientID)
                .WithRedirectUri(specialRedirectUri ?? $"msal{ClientID}://auth")
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .Build();

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
