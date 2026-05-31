using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nanpla.Models;
using Microsoft.Maui.Controls;

namespace Nanpla.ViewModels
{
    public class GameViewModel : BindableObject, IQueryAttributable
    {
        private NanplaBoard? _board;
        private bool _isNoteMode;
        private bool _isGameWonInternal;
        private bool _isGameStarted;
        private bool _isShowCongratsModal;
        private bool _isShowOnlyImage;
        private NanplaCell? _selectedCell;
        private ImageSource? _backgroundImageSource;
        private string _currentImageName = string.Empty;
        private bool _isBlurred = true;
        private string _difficultyText = string.Empty;
        private string _congratsMessage = string.Empty;

        public string CongratsMessage
        {
            get => _congratsMessage;
            set
            {
                if (_congratsMessage != value)
                {
                    _congratsMessage = value;
                    OnPropertyChanged(nameof(CongratsMessage));
                }
            }
        }

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

        public ObservableCollection<NanplaCell> FlattenedCells { get; } = new ObservableCollection<NanplaCell>();

        public bool IsNoteMode
        {
            get => _isNoteMode;
            set
            {
                if (_isNoteMode != value)
                {
                    _isNoteMode = value;
                    OnPropertyChanged(nameof(IsNoteMode));
                    OnPropertyChanged(nameof(NoteModeText));
                }
            }
        }

        public string NoteModeText => IsNoteMode ? "Notes: ON" : "Notes: OFF";

        public bool IsGameWonInternal
        {
            get => _isGameWonInternal;
            set
            {
                if (_isGameWonInternal != value)
                {
                    _isGameWonInternal = value;
                    OnPropertyChanged(nameof(IsGameWonInternal));
                }
            }
        }

        public bool IsGameStarted
        {
            get => _isGameStarted;
            set
            {
                if (_isGameStarted != value)
                {
                    _isGameStarted = value;
                    OnPropertyChanged(nameof(IsGameStarted));
                }
            }
        }

        public bool IsShowCongratsModal
        {
            get => _isShowCongratsModal;
            set
            {
                if (_isShowCongratsModal != value)
                {
                    _isShowCongratsModal = value;
                    OnPropertyChanged(nameof(IsShowCongratsModal));
                }
            }
        }

        public bool IsShowOnlyImage
        {
            get => _isShowOnlyImage;
            set
            {
                if (_isShowOnlyImage != value)
                {
                    _isShowOnlyImage = value;
                    OnPropertyChanged(nameof(IsShowOnlyImage));
                }
            }
        }

        public NanplaCell? SelectedCell
        {
            get => _selectedCell;
            set
            {
                if (_selectedCell != value)
                {
                    _selectedCell = value;
                    OnPropertyChanged(nameof(SelectedCell));
                }
            }
        }

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

        public bool IsBlurred
        {
            get => _isBlurred;
            set
            {
                if (_isBlurred != value)
                {
                    _isBlurred = value;
                    OnPropertyChanged(nameof(IsBlurred));
                }
            }
        }

        public string DifficultyText
        {
            get => _difficultyText;
            set
            {
                if (_difficultyText != value)
                {
                    _difficultyText = value;
                    OnPropertyChanged(nameof(DifficultyText));
                }
            }
        }

        public ICommand StartGameCommand { get; }
        public ICommand InputNumberCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ToggleNoteModeCommand { get; }
        public ICommand SelectCellCommand { get; }
        public ICommand GoToTitleCommand { get; }
        public ICommand ShowImageOnlyCommand { get; }
        public ICommand PageTapCommand { get; }

        public GameViewModel()
        {
            StartGameCommand = new Command<Difficulty>(StartGame);
            InputNumberCommand = new Command<string>(InputNumber);
            ClearCommand = new Command(Clear);
            ToggleNoteModeCommand = new Command(ToggleNoteMode);
            SelectCellCommand = new Command<NanplaCell>(SelectCell);
            GoToTitleCommand = new Command(GoToTitle);
            ShowImageOnlyCommand = new Command(ShowImageOnly);
            PageTapCommand = new Command(OnPageTap);
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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Difficulty", out var diffObj) && diffObj != null)
            {
                if (int.TryParse(diffObj.ToString(), out int diffVal))
                {
                    StartGame((Difficulty)diffVal);
                }
            }
        }

        private void StartGame(Difficulty difficulty)
        {
            try
            {
                if (SelectedCell != null)
                {
                    SelectedCell.IsSelected = false;
                }
                SelectedCell = null;

                _board = new NanplaBoard();
                _board.GenerateBoard(difficulty);

                var allImages = AlbumManager.GetAllImages();
                if (allImages != null && allImages.Count > 0)
                {
                    var unlockedImages = AlbumManager.GetUnlockedImages();
                    var lockedImages = allImages.Where(img => !unlockedImages.Contains(img)).ToList();

                    var rand = new Random();
                    if (lockedImages.Count > 0)
                    {
                        // まだ獲得していない画像から優先的にランダム選択
                        _currentImageName = lockedImages[rand.Next(lockedImages.Count)];
                    }
                    else
                    {
                        // すべて獲得済みの場合は全画像からランダム選択
                        _currentImageName = allImages[rand.Next(allImages.Count)];
                    }

                    _board.BackgroundImage = _currentImageName;
                    LoadBackgroundImage(_currentImageName);
                }
                else
                {
                    BackgroundImageSource = null;
                    _currentImageName = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting game or loading background image: {ex.Message}");
                BackgroundImageSource = null;
                _currentImageName = string.Empty;
            }

            IsBlurred = true;
            DifficultyText = difficulty switch
            {
                Difficulty.Easy => "Difficulty: Easy",
                Difficulty.Medium => "Difficulty: Medium",
                Difficulty.Hard => "Difficulty: Hard",
                _ => "Difficulty: Normal"
            };

            FlattenedCells.Clear();
            if (_board != null)
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        FlattenedCells.Add(_board.Cells[r, c]);
                    }
                }
            }

            IsGameStarted = true;
            IsGameWonInternal = false;
            IsNoteMode = false;
            IsShowCongratsModal = false;
            IsShowOnlyImage = false;
        }

        private void LoadBackgroundImage(string imageName)
        {
            try
            {
                if (string.IsNullOrEmpty(imageName))
                {
                    BackgroundImageSource = null;
                    return;
                }

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    BackgroundImageSource = ImageSource.FromStream((token) => 
                        FileSystem.OpenAppPackageFileAsync($"photo/{imageName}")
                    );
                }
                else
                {
                    var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo", imageName);
                    BackgroundImageSource = ImageSource.FromFile(fullPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load game background: {ex.Message}");
                BackgroundImageSource = null;
            }
        }

        private void SelectCell(NanplaCell cell)
        {
            TriggerHapticFeedback();
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
            }
            SelectedCell = cell;
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = true;
            }
        }

        private void InputNumber(string numberStr)
        {
            TriggerHapticFeedback();
            if (SelectedCell == null) return;
            if (SelectedCell.IsOriginal) return;
            if (!int.TryParse(numberStr, out int number)) return;

            if (IsNoteMode)
            {
                if (SelectedCell.GetNote(number))
                {
                    SelectedCell.RemoveNote(number);
                }
                else
                {
                    SelectedCell.AddNote(number);
                }
            }
            else
            {
                if (SelectedCell.Value == number)
                {
                    SelectedCell.Value = 0;
                    SelectedCell.ClearNotes();
                }
                else
                {
                    SelectedCell.Value = number;
                    SelectedCell.ClearNotes();
                }
            }

            CheckGameState();
        }

        private void Clear()
        {
            TriggerHapticFeedback();
            if (SelectedCell == null) return;
            if (SelectedCell.IsOriginal) return;

            if (IsNoteMode)
            {
                SelectedCell.ClearNotes();
            }
            else
            {
                SelectedCell.Value = 0;
                SelectedCell.ClearNotes();
            }

            CheckGameState();
        }

        private void ToggleNoteMode()
        {
            TriggerHapticFeedback();
            IsNoteMode = !IsNoteMode;
        }

        private void CheckGameState()
        {
            if (IsGameStarted && _board != null)
            {
                _board.ValidateBoard();

                bool won = _board.CheckWin();
                if (won != _isGameWonInternal)
                {
                    IsGameWonInternal = won;
                    if (won)
                    {
                        IsBlurred = false;
                        if (SelectedCell != null)
                        {
                            SelectedCell.IsSelected = false;
                        }
                        SelectedCell = null;

                        if (!string.IsNullOrEmpty(_currentImageName))
                        {
                            var unlockedBefore = AlbumManager.GetUnlockedImages();
                            bool isAlreadyUnlocked = unlockedBefore.Contains(_currentImageName);

                            AlbumManager.UnlockImage(_currentImageName);

                            var allImages = AlbumManager.GetAllImages();
                            var unlockedAfter = AlbumManager.GetUnlockedImages();

                            if (unlockedAfter.Count >= allImages.Count)
                            {
                                if (unlockedBefore.Count < allImages.Count)
                                {
                                    // ちょうどこのクリアでコンプリートした場合
                                    CongratsMessage = "Congratulations!\nYou have unlocked the final image and COMPLETED the entire album!";
                                }
                                else
                                {
                                    // すでにコンプリート済みの場合
                                    CongratsMessage = "Congratulations!\nYou cleared the puzzle!";
                                }
                            }
                            else
                            {
                                // 通常の新規解放の場合
                                CongratsMessage = "Congratulations!\nThe background image has been unlocked and added to your album.";
                            }
                        }
                        else
                        {
                            CongratsMessage = "Congratulations!\nYou cleared the puzzle!";
                        }

                        IsShowCongratsModal = true;
                        IsShowOnlyImage = false;
                    }
                }
            }
        }

        private void ShowImageOnly()
        {
            IsShowCongratsModal = false;
            IsShowOnlyImage = true;
        }

        private void OnPageTap()
        {
            if (IsShowOnlyImage)
            {
                GoToTitle();
            }
        }

        private async void GoToTitle()
        {
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
            }
            SelectedCell = null;
            await Shell.Current.GoToAsync("..");
        }
    }
}
