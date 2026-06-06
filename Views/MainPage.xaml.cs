using Microsoft.Maui.Controls;
using Nanpla.ViewModels;
using Nanpla.Models;
#if ANDROID || IOS
using Plugin.MauiMTAdmob.Controls;
#endif

namespace Nanpla.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(GameViewModel viewModel) : this()
        {
            BindingContext = viewModel;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

#if ANDROID || IOS
            if (BindingContext is GameViewModel viewModel)
            {
                var adView = new MTAdView
                {
                    AdsId = viewModel.AdUnitId,
                    HeightRequest = 50,
                    HorizontalOptions = LayoutOptions.Center
                };
                AdContainer.Content = adView;
            }
#endif
        }

        private void OnCellTapped(object sender, TappedEventArgs e)
        {
            if (BindingContext is GameViewModel vm && sender is BindableObject bo && bo.BindingContext is NanplaCell cell)
            {
                vm.SelectCellCommand.Execute(cell);
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && height > 0)
            {
                // 余白（PagePadding, GridPadding, BorderPadding等 94px）に、左右の安全バッファ約4pxずつ（計8px）を含めて98pxとする
                double margin = 98;
                double availableWidth = width - margin;
                
                // 縦方向のスペース（ヘッダー・フッター広告・ボタン領域）の確保: 250px
                double availableHeight = height - 250;
                
                // 縦横の制限値の小さい方に合わせる（スクエア形状を維持）
                double size = Math.Min(availableWidth, availableHeight);
                
                // デスクトップやタブレットで見苦しく巨大化しないよう、最大幅を360pxに制限
                if (size > 360)
                {
                    size = 360;
                }
                
                // 9マスのセルが均等に並び、描画の微小な小数点端数のズレを防ぐため、9の倍数に丸める
                size = Math.Floor(size / 9) * 9;
                
                if (size > 0)
                {
                    BoardGrid.WidthRequest = size;
                    BoardGrid.HeightRequest = size;
                    BordersGrid.WidthRequest = size;
                    BordersGrid.HeightRequest = size;
                }
            }
        }
    }
}
