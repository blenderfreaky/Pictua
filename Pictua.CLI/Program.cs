using Microsoft.Extensions.Logging;
using Pictua.OneDrive;
using System.Threading.Tasks;
using Pictua.GoogleDrive;
using System;

namespace Pictua.CLI
{
    public static class Program
    {
        public static async Task Main()
        {
            var logger = LoggerFactory.Create(options =>
                options.AddConsole());

            var client = Client.Create(new FilePathConfig("Pictua"), logger.CreateLogger<Client>());
            //var server = GoogleDriveServer.Create(FilePathConfig.Server, logger.CreateLogger<GoogleDriveServer>());
            var server = await OneDriveServer.Create(FilePathConfig.Server, logger.CreateLogger<OneDriveServer>()).ConfigureAwait(false);

            await client.SyncAsync(server).ConfigureAwait(false);
        }
    }
}
