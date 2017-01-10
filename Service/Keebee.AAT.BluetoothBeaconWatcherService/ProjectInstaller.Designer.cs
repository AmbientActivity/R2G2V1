namespace Keebee.AAT.BluetoothBeaconWatcherService
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
            this.bluetoothBeaconWatcherServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.bluetoothBeaconWatcherServiceInstaller});
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // bluetoothBeaconWatcherServiceInstaller
            // 
            this.bluetoothBeaconWatcherServiceInstaller.Description = "Processes all Bluetooth Beacon Advertisements and hands them to the State Machine" +
    " Service";
            this.bluetoothBeaconWatcherServiceInstaller.DisplayName = "Keebee AAT Bluetooth Beacon Watcher Service";
            this.bluetoothBeaconWatcherServiceInstaller.ServiceName = "BluetoothBeaconWatcherService";
            this.bluetoothBeaconWatcherServiceInstaller.ServicesDependedOn = new string[] {
        "MSMQ",
        "StateMachineService"};
            this.bluetoothBeaconWatcherServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.bluetoothBeaconWatcherServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.bluetoothAdvertisementInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller bluetoothBeaconWatcherServiceInstaller;
    }
}