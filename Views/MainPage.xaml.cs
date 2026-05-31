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
    }
}
