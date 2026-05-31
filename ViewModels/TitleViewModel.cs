using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Nanpla.Models;

namespace Nanpla.ViewModels
{
    public class TitleViewModel : BindableObject
    {
        private ImageSource? _backgroundImageSource;

        public ImageSource? BackgroundImageSource
        {
            get => _backgroundImageSource;
            set
            {
                if (_backgroundImageSource != value)
                {
                    _backgroundImageSource = value;
                    OnPropertyChanged(nameof(BackgroundImageSource));
                }
            }
        }

        public ICommand StartGameCommand { get; }
        public ICommand OpenAlbumCommand { get; }

        public string AdUnitId
        {
            get
            {
#if DEBUG
                return "ca-app-pub-3940256099942544/6300978111"; // Google Test Ad Unit ID for Banner
#else
                return "ca-app-pub-7695104877965082/7697740776"; // Production Ad Unit ID for Banner
#endif
            }
        }

        public TitleViewModel()
        {
            StartGameCommand = new Command<Difficulty>(StartGame);
            OpenAlbumCommand = new Command(OpenAlbum);
            
            LoadBackgroundImage();

            // Trigger asset index generation for Android/Fallback mode on startup
            System.Threading.Tasks.Task.Run(() => AlbumManager.GetAllImages());
        }

        private void LoadBackgroundImage()
        {
            try
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    BackgroundImageSource = ImageSource.FromStream((token) => 
                        FileSystem.OpenAppPackageFileAsync("photo/title.jpg")
                    );
                }
                else
                {
                    var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo", "title.jpg");
                    BackgroundImageSource = ImageSource.FromFile(fullPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load title background: {ex.Message}");
            }
        }

        private async void StartGame(Difficulty difficulty)
        {
            // Navigate to MainPage with selected difficulty
            await Shell.Current.GoToAsync($"MainPage?Difficulty={(int)difficulty}");
        }

        private async void OpenAlbum()
        {
            // Navigate to AlbumPage
            await Shell.Current.GoToAsync("AlbumPage");
        }
    }
}
