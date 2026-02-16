using Syncfusion.Licensing;
namespace YoutubeDownloader
{
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense("MDAxQDMyMzIyZTMwMmUzMGZrMFNJK01LMk5jdUc4dzVMYmlJU250V05PQzVWclVOSE5aeFpHMEdhNDg9");
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new AppShell());
        }
    }
}