namespace Nanpla
{
    public partial class MainApp : Application
    {
        public MainApp()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
#if WINDOWS
            window.Width = 650;
            window.Height = 900;
#endif
            return window;
        }
    }
}
