using Microsoft.Extensions.Logging;
using Pictua.OneDrive;
using System.Threading.Tasks;
using Pictua.GoogleDrive;

namespace Pictua.CLI
{
    public static class Program
    {
        public static async Task Main()
        {
            var logger = LoggerFactory.Create(options =>
                options.AddConsole());

            var client = Client.Create(new FilePathConfig("Pictua"), logger.CreateLogger<Client>());
            var gdServer = GoogleDriveServer.Create(FilePathConfig.Server, logger.CreateLogger<GoogleDriveServer>());
            var odServer = OneDriveServer.Create(FilePathConfig.Server, logger.CreateLogger<OneDriveServer>());

            await client.SyncAsync(gdServer).ConfigureAwait(false);
        }
    }
}
