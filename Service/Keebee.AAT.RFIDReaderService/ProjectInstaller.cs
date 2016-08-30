using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Keebee.AAT.RFIDReaderService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(rfidReaderServiceInstaller.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
