using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Nanpla.Models;

namespace Nanpla.ViewModels
{
    public class AlbumViewModel : BindableObject
    {
        public class AlbumItem
        {
            public string FileName { get; set; } = string.Empty;
            public ImageSource? ImageSource { get; set; }
            public bool IsUnlocked { get; set; }
            public string NumberText { get; set; } = string.Empty;
        }

        private bool _isPreviewVisible;
        private ImageSource? _previewImageSource;

        public ObservableCollection<AlbumItem> AlbumItems { get; } = new ObservableCollection<AlbumItem>();

        public bool IsPreviewVisible
        {
            get => _isPreviewVisible;
            set
            {
                if (_isPreviewVisible != value)
                {
                    _isPreviewVisible = value;
                    OnPropertyChanged(nameof(IsPreviewVisible));
                }
            }
        }

        public ImageSource? PreviewImageSource
        {
            get => _previewImageSource;
            set
            {
                if (_previewImageSource != value)
                {
                    _previewImageSource = value;
                    OnPropertyChanged(nameof(PreviewImageSource));
                }
            }
        }

        public ICommand GoToTitleCommand { get; }
        public ICommand PreviewImageCommand { get; }
        public ICommand ClosePreviewCommand { get; }

        public AlbumViewModel()
        {
            GoToTitleCommand = new Command(GoToTitle);
            PreviewImageCommand = new Command<AlbumItem>(PreviewImage);
            ClosePreviewCommand = new Command(ClosePreview);
            LoadAlbum();
        }

        private async void LoadAlbum()
        {
            AlbumItems.Clear();
            var items = await Task.Run(() =>
            {
                var allImages = AlbumManager.GetAllImages();
                var unlockedImages = AlbumManager.GetUnlockedImages();
                var list = new List<AlbumItem>();
                int index = 1;
                foreach (var img in allImages)
                {
                    var isUnlocked = unlockedImages.Contains(img);
                    ImageSource? imageSrc = null;

                    if (isUnlocked)
                    {
                        string thumbName = $"thumb_{img}";
                        if (DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            string assetPath = $"photo/{thumbName}";
                            imageSrc = ImageSource.FromStream((token) => 
                                FileSystem.OpenAppPackageFileAsync(assetPath)
                            );
                        }
                        else
                        {
                            var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo", thumbName);
                            imageSrc = ImageSource.FromFile(fullPath);
                        }
                    }

                    list.Add(new AlbumItem
                    {
                        FileName = img,
                        ImageSource = imageSrc,
                        IsUnlocked = isUnlocked,
                        NumberText = $"No. {index++}"
                    });
                }
                return list;
            });

            foreach (var item in items)
            {
                AlbumItems.Add(item);
            }
        }

        private void TriggerHapticFeedback()
        {
            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Haptic feedback failed: {ex.Message}");
            }
        }

        private void PreviewImage(AlbumItem item)
        {
            TriggerHapticFeedback();
            if (item != null && item.IsUnlocked)
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    string assetPath = $"photo/{item.FileName}";
                    PreviewImageSource = ImageSource.FromStream((token) => 
                        FileSystem.OpenAppPackageFileAsync(assetPath)
                    );
                }
                else
                {
                    var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo", item.FileName);
                    PreviewImageSource = ImageSource.FromFile(fullPath);
                }
                IsPreviewVisible = true;
            }
        }

        private void ClosePreview()
        {
            TriggerHapticFeedback();
            IsPreviewVisible = false;
        }

        private async void GoToTitle()
        {
            TriggerHapticFeedback();
            await Shell.Current.GoToAsync("..");
        }
    }
}
