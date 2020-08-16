namespace DriveQuickstart
{
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using Pictua.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class GoogleDriveServer : Server
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static readonly string[] Scopes = { DriveService.Scope.DriveAppdata };
        private const string ApplicationName = "Pictua";

        private DriveService _service;

        public GoogleDriveServer()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                const string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = _service.Files.List();

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

            Console.WriteLine("Files:");

            if (files == null || files.Count == 0)
            {
                Console.WriteLine("No files found.");
                return;
            }

            foreach (var file in files)
            {
                Console.WriteLine($"{file.Name} ({file.Id})");
            }
        }

        public override Task CommitAsync()
        {
        }

        public override Task DeleteLocalFileAsync(ServerFile file) => throw new NotImplementedException();
        public override Task LockAsync()
        {
            _service.Files.Watch(new Google.Apis.Drive.v3.Data.Channel(), )
        }

        public override Task<ConcreteFile> PullFileAsync(ServerFile file, IClientIdentity puller) => throw new NotImplementedException();
        public override Task UnlockAsync() => throw new NotImplementedException();
        protected override Task PushFileAsync(string origin, ServerFile target) => throw new NotImplementedException();
    }
}
