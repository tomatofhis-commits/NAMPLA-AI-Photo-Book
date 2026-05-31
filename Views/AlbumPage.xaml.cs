using System;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using Nanpla.ViewModels;

namespace Nanpla.Views
{
    public partial class AlbumPage : ContentPage
    {
        private bool _isUpdatingFromScroll = false;
        private bool _isUpdatingFromSlider = false;

        public AlbumPage()
        {
            InitializeComponent();
        }

        public AlbumPage(AlbumViewModel viewModel) : this()
        {
            BindingContext = viewModel;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is AlbumViewModel viewModel)
            {
                viewModel.AlbumItems.CollectionChanged += AlbumItems_CollectionChanged;
                UpdateSliderMaximum();
            }
        }

        private void AlbumItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSliderMaximum();
        }

        private void UpdateSliderMaximum()
        {
            if (BindingContext is AlbumViewModel viewModel)
            {
                int count = viewModel.AlbumItems.Count;
                if (count > 0)
                {
                    AlbumSlider.Maximum = count - 1;
                    IndexLabel.Text = $"1 / {count}";
                }
                else
                {
                    AlbumSlider.Maximum = 0;
                    IndexLabel.Text = "0 / 0";
                }
            }
        }

        private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_isUpdatingFromSlider)
                return;

            _isUpdatingFromScroll = true;
            try
            {
                int firstIndex = e.FirstVisibleItemIndex;
                if (BindingContext is AlbumViewModel viewModel)
                {
                    int count = viewModel.AlbumItems.Count;
                    if (count > 0 && firstIndex >= 0 && firstIndex < count)
                    {
                        AlbumSlider.Value = firstIndex;
                        IndexLabel.Text = $"{firstIndex + 1} / {count}";
                    }
                }
            }
            finally
            {
                _isUpdatingFromScroll = false;
            }
        }

        private void AlbumSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_isUpdatingFromScroll)
                return;

            _isUpdatingFromSlider = true;
            try
            {
                int targetIndex = (int)Math.Round(e.NewValue);
                if (BindingContext is AlbumViewModel viewModel)
                {
                    int count = viewModel.AlbumItems.Count;
                    if (count > 0 && targetIndex >= 0 && targetIndex < count)
                    {
                        AlbumCollectionView.ScrollTo(targetIndex, position: ScrollToPosition.MakeVisible, animate: false);
                        IndexLabel.Text = $"{targetIndex + 1} / {count}";
                    }
                }
            }
            finally
            {
                _isUpdatingFromSlider = false;
            }
        }
    }
}

