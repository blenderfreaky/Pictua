using Pictua.Tags;
using Plugin.FilePicker;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pictua.XFUI.ViewModels
{
    public class GalleryViewModel : ReactiveObject
    {
        public GalleryViewModel()
        {
            ItemTappedCommand = ReactiveCommand.Create(() =>
            {
                if (LastTappedItem is ItemModel item)
                {
                    Debug.WriteLine($"Tapped {item.Title}");
                }
            });

            UploadFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var file = await CrossFilePicker.Current.PickFile().ConfigureAwait(false);
                if (file == null) return;

                var client = App.Current.ViewModel.Client;

                var descriptor = client.AddFile(Path.GetExtension(file.FileName), file.FileName, file.GetStream());

                if (descriptor == null) return;

                Device.BeginInvokeOnMainThread(() =>
                    Items.Add(new ItemModel
                    {
                        Title = file.FileName,
                        ImageUrl = client.FilePaths.GetFilePath(descriptor.Value)
                    }));
            });

            ReloadData();
        }

        public void ReloadData()
        {
            var client = App.Current.ViewModel.Client;

            Items = new ObservableCollection<ItemModel>(
                client.State.Files
                .Select(x => new ItemModel
                {
                    Title = x.Metadata?.Tags
                        .Select(x => (x as StringTag?)?.SubTags)
                        .First(x => x?[0] == "Title")
                        [1],
                    ImageUrl = client.FilePaths.GetFilePath(x.Descriptor)
                }));
        }

        public ICommand UploadFileCommand { get; }

        public ICommand ItemTappedCommand { get; }

        [Reactive]
        public object LastTappedItem { get; set; }

        [Reactive]
        public ObservableCollection<ItemModel> Items { get; set; }

        public class ItemModel
        {
            [Reactive]
            public string ImageUrl { get; set; }

            [Reactive]
            public string Title { get; set; }
        }
    }
}