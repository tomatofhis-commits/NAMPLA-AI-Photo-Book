using Microsoft.Maui.Controls;
using Nanpla.ViewModels;
#if ANDROID || IOS
using Plugin.MauiMTAdmob.Controls;
#endif

namespace Nanpla.Views
{
    public partial class TitlePage : ContentPage
    {
        public TitlePage()
        {
            InitializeComponent();
        }

        public TitlePage(TitleViewModel viewModel) : this()
        {
            BindingContext = viewModel;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

#if ANDROID || IOS
            if (BindingContext is TitleViewModel viewModel)
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
    }
}
