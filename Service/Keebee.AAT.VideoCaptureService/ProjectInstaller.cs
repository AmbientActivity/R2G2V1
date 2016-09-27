using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Keebee.AAT.VideoCaptureService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void videoCaptureServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(videoCaptureServiceInstaller.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
