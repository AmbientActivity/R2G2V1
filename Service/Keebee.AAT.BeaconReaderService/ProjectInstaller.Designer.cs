namespace Keebee.AAT.BeaconReaderService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.beaconReaderInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.beaconReaderInstaller});
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // beaconReaderInstaller
            // 
            this.beaconReaderInstaller.Description = "Processes all iBeacon events via Bluetooth and hands them to the State Machine Se" +
    "rvice";
            this.beaconReaderInstaller.DisplayName = "Keebee AAT Beacon Reader Service";
            this.beaconReaderInstaller.ServiceName = "BeaconReaderService";
            this.beaconReaderInstaller.ServicesDependedOn = new string[] {
        "MSMQ",
        "StateMachineService"};
            this.beaconReaderInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.beaconReaderInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.beaconReaderInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller beaconReaderInstaller;
    }
}