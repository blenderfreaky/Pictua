using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using Pictua;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GDFile = Google.Apis.Drive.v3.Data.File;

namespace DriveQuickstart
{
    public class GoogleDriveServer : Server
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static readonly string[] Scopes = { DriveService.Scope.DriveAppdata };

        private readonly DriveService _service;

        public GoogleDriveServer(FilePathConfig filePaths, ILogger<GoogleDriveServer> logger) : base(filePaths, logger)
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
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
                ApplicationName = "Pictua",
            });
        }

        protected async Task<string?> GetFileId(string path)
        {
            // TODO: Add proper support for folders
            var name = Path.GetFileName(path);

            var listRequest = _service.Files.List();
            listRequest.Q = $"name = '{name.Replace("'", "\'")}'";
            var fileList = await listRequest.ExecuteAsync().ConfigureAwait(false);
            var file = fileList.Files.FirstOrDefault(x => x.Name == name);
            return file?.Id;
        }

        protected override async Task<bool> DeleteAsync(string path)
        {
            var id = await GetFileId(path).ConfigureAwait(false);
            if (id == null) return false;

            await _service.Files.Delete(id).ExecuteAsync().ConfigureAwait(false);

            return true;
        }

        protected override async Task<bool> FileExistsAsnyc(string path)
        {
            var id = await GetFileId(path).ConfigureAwait(false);
            if (id == null) return false;

            var file = await _service.Files.Get(id).ExecuteAsync().ConfigureAwait(false);

            return file != null;
        }

        protected override async Task<bool> DownloadAsync(Stream stream, string originPath)
        {
            var id = await GetFileId(originPath).ConfigureAwait(false);
            if (id == null) return false;

            await _service.Files.Get(id).DownloadAsync(stream).ConfigureAwait(false);

            return true;
        }

        protected override async Task<bool> UploadAsync(Stream stream, string targetPath)
        {
            var extension = Path.GetExtension(targetPath);
            var fileMetadata = new GDFile()
            {
                Name = Path.GetFileName(targetPath),
                FileExtension = extension
            };

            var request = _service.Files.Create(fileMetadata, stream, MimeTypesMap.GetMimeType(extension));
            await request.UploadAsync().ConfigureAwait(false);

            return true;
        }
    }
}
