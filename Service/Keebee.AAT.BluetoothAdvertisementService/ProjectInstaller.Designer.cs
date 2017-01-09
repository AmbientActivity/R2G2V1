namespace Keebee.AAT.BluetoothAdvertisementService
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
            this.bluetoothAdvertisementServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // bluetoothAdvertisementServiceInstaller
            // 
            this.bluetoothAdvertisementServiceInstaller.Description = "Processes all Bluetooth Advertisements and hands them to the State Machine Servic" +
    "e";
            this.bluetoothAdvertisementServiceInstaller.DisplayName = "Keebee AAT Bluetooth Advertisement Service";
            this.bluetoothAdvertisementServiceInstaller.ServiceName = "BluetoothAdvertisementService";
            this.bluetoothAdvertisementServiceInstaller.ServicesDependedOn = new string[] {
        "MSMQ",
        "StateMachineService"};
            this.bluetoothAdvertisementServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.bluetoothAdvertisementServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.bluetoothAdvertisementInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller bluetoothAdvertisementServiceInstaller;
    }
}