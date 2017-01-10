using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Keebee.AAT.BluetoothBeaconWatcherService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void bluetoothAdvertisementInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(bluetoothBeaconWatcherServiceInstaller.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
