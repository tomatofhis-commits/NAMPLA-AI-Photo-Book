using Microsoft.Maui.Controls;

namespace Nanpla
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // 逕ｻ髱｢驕ｷ遘ｻ逕ｨ縺ｮ繝ｫ繝ｼ繝育匳骭ｲ
            Routing.RegisterRoute("MainPage", typeof(Views.MainPage));
            Routing.RegisterRoute("AlbumPage", typeof(Views.AlbumPage));
        }
    }
}
