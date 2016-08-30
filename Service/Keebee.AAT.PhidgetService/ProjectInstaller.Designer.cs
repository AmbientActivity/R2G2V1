namespace Keebee.AAT.PhidgetService
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
            this.phidgetServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.phidgetServiceInstaller});
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // phidgetServiceInstaller
            // 
            this.phidgetServiceInstaller.Description = "Keebee AAT Phidget Service";
            this.phidgetServiceInstaller.DisplayName = "Keebee AAT Phidget Service";
            this.phidgetServiceInstaller.ServiceName = "PhidgetService";
            this.phidgetServiceInstaller.ServicesDependedOn = new string[] {
        "MSMQ"};
            this.phidgetServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.phidgetServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.phidgetServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller phidgetServiceInstaller;
    }
}