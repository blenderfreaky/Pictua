using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Linq;
using System.Threading.Tasks;

namespace Pictua.OneDrive
{
    public class OneDrive
    {
        private readonly IPublicClientApplication _app;
        private readonly IAuthenticationProvider _authProvider;
        private readonly GraphServiceClient _graphClient;

        private IDriveItemRequestBuilder AppRoot => _graphClient.Me.Drive.Special.AppRoot;

        protected OneDrive(IPublicClientApplication app, IAuthenticationProvider authProvider, GraphServiceClient graphClient)
        {
            _app = app;
            _authProvider = authProvider;
            _graphClient = graphClient;
        }

        public async static OneDrive CreateAsync(string clientId, params string[] scopes)
        {
            var app = PublicClientApplicationBuilder
                               .Create(clientId)
                               .Build();

            //var accounts = await app.GetAccountsAsync().ConfigureAwait(false);

            //AuthenticationResult result;
            //try
            //{
            //    result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
            //                .ExecuteAsync().ConfigureAwait(false);
            //}
            //catch (MsalUiRequiredException)
            //{
            //    result = await app.AcquireTokenInteractive(scopes)
            //                .ExecuteAsync().ConfigureAwait(false);
            //}

            var authProvider = new InteractiveAuthenticationProvider(app, scopes);

            var graphClient = new GraphServiceClient(authProvider);

            return new OneDrive(app, authProvider, graphClient);
        }

        public async Task Write(string path)
        {
            AppRoot.ItemWithPath("")
        }

        // Files.ReadWrite.AppFolder
    }
}
