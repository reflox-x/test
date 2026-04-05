using Microsoft.Extensions.DependencyInjection;

namespace pawledger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            bool savedDarkMode = Preferences.Default.Get("darkmode", false);
            UserAppTheme = savedDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}