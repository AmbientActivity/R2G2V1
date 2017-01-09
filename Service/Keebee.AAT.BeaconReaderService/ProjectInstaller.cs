using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Keebee.AAT.BeaconReaderService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void beaconReaderInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(beaconReaderInstaller.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
