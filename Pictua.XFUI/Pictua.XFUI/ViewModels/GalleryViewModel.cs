using Pictua.Tags;
using Plugin.FilePicker;
using Plugin.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                AddFile(file.FileName, file.GetStream());
                App.Current.ViewModel.Client.Commit();
            });

            UploadPhotoCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(false);
                if (file == null) return;
                AddFile(file.AlbumPath, file.GetStream());
                App.Current.ViewModel.Client.Commit();
            });

            UploadPhotosCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var files = await CrossMedia.Current.PickPhotosAsync().ConfigureAwait(false);
                if (files == null) return;
                Parallel.ForEach(files, file => AddFile(file.AlbumPath, file.GetStream()));
                App.Current.ViewModel.Client.Commit();
            });

            UploadVideoCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var file = await CrossMedia.Current.PickVideoAsync().ConfigureAwait(false);
                if (file == null) return;
                AddFile(file.AlbumPath, file.GetStream());
                App.Current.ViewModel.Client.Commit();
            });

            ReloadData();
        }

        private void AddFile(string fileName, Stream stream)
        {
            try
            {
                var client = App.Current.ViewModel.Client;

                var descriptor = client.AddFile(Path.GetExtension(fileName), fileName, stream);

                if (descriptor == null) return;

                Device.BeginInvokeOnMainThread(() =>
                    Items.Add(new ItemModel
                    {
                        Title = fileName,
                        ImageUrl = client.FilePaths.GetFilePath(descriptor.Value)
                    }));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
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
        public ICommand UploadPhotoCommand { get; }
        public ICommand UploadPhotosCommand { get; }
        public ICommand UploadVideoCommand { get; }

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