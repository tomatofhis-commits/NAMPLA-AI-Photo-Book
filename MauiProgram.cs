using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Nanpla.Views;
using Nanpla.ViewModels;
#if ANDROID || IOS
using Plugin.MauiMTAdmob;
#endif

namespace Nanpla
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<MainApp>()
#if ANDROID || IOS
                .UseMauiMTAdmob()
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Pages & ViewModels 繧・DI 逋ｻ骭ｲ
            builder.Services.AddSingleton<TitlePage>();
            builder.Services.AddSingleton<TitleViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<GameViewModel>();

            builder.Services.AddTransient<AlbumPage>();
            builder.Services.AddTransient<AlbumViewModel>();

            return builder.Build();
        }
    }
}
