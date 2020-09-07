using Plugin.FilePicker;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

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
                    Debug.WriteLine($"Tapped {item.FileName}");
                }
            });

            UploadFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var file = await CrossFilePicker.Current.PickFile().ConfigureAwait(false);
                if (file == null) return;
                App.Current.ViewModel.Client.AddFile(Path.GetExtension(file.FileName), file.GetStream());
            });

            ReloadData();
        }

        public void ReloadData()
        {
            var list = new ObservableCollection<ItemModel>();

            string[] images = {
                "https://farm9.staticflickr.com/8625/15806486058_7005d77438.jpg",
                "https://farm5.staticflickr.com/4011/4308181244_5ac3f8239b.jpg",
                "https://farm8.staticflickr.com/7423/8729135907_79599de8d8.jpg",
                "https://farm3.staticflickr.com/2475/4058009019_ecf305f546.jpg",
                "https://farm6.staticflickr.com/5117/14045101350_113edbe20b.jpg",
                "https://farm2.staticflickr.com/1227/1116750115_b66dc3830e.jpg",
                "https://farm8.staticflickr.com/7351/16355627795_204bf423e9.jpg",
                "https://farm1.staticflickr.com/44/117598011_250aa8ffb1.jpg",
                "https://farm8.staticflickr.com/7524/15620725287_3357e9db03.jpg",
                "https://farm9.staticflickr.com/8351/8299022203_de0cb894b0.jpg",
            };

            int number = 0;
            for (int n = 0; n < 20; n++)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    number++;
                    var item = new ItemModel()
                    {
                        ImageUrl = images[i],
                        FileName = string.Format("image_{0}.jpg", number),
                    };

                    list.Add(item);
                }
            }

            Items = list;
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
            public string FileName { get; set; }
        }
    }
}