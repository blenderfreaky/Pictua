using Microsoft.Extensions.Logging;
using Pictua.OneDrive;
using System.Diagnostics;

namespace Pictua.AvaloniaUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Client Client { get; }
        public Server Server { get; }

        public MainWindowViewModel()
        {
            var loggerFactory = LoggerFactory.Create(options => options.AddTraceSource(new SourceSwitch("Pictua")));

            Client = Client.Create(FilePathConfig.Client, loggerFactory.CreateLogger<Client>());
            Server = OneDriveServer.Create(FilePathConfig.Client, loggerFactory.CreateLogger<OneDriveServer>());
        }
    }
}
