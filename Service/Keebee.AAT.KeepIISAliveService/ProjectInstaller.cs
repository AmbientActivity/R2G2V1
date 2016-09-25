using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Keebee.AAT.KeepIISAliveService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void keepIISAliveServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(keepIISAliveServiceInstaller.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
