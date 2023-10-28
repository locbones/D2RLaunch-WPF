using System.Windows.Threading;
using log4net;

namespace D2RLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ILog _logger = LogManager.GetLogger(typeof(App));
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWH9feXRRQmReUkF2VkE=");
            InitializeComponent();
        }

        private void OnUnhandledDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger?.Error(e.Exception);
            _logger?.Error(e.Exception.Message);

            e.Handled = true;
        }
    }
}
