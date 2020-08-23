using Microsoft.Extensions.Logging;
using Pictua.OneDrive;
using System.Threading.Tasks;

namespace Pictua.CLI
{
    public static class Program
    {
        public static async Task Main()
        {
            var logger = LoggerFactory.Create(options =>
                options.AddConsole());

            var client = Client.Create(new FilePathConfig("Pictua"), logger.CreateLogger<Client>());
            var server = OneDriveServer.Create(FilePathConfig.Server, logger.CreateLogger<OneDriveServer>());

            await client.SyncAsync(server).ConfigureAwait(false);
        }
    }
}
