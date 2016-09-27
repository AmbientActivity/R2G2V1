namespace Keebee.AAT.KeepIISAliveService
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
            this.keepIISAliveServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.keepIISAliveServiceInstaller});
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // keepIISAliveServiceInstaller
            // 
            this.keepIISAliveServiceInstaller.Description = "Keeps the Operations API and Administrator Interface alive";
            this.keepIISAliveServiceInstaller.DisplayName = "Keebee AAT Keep IIS Alive Service";
            this.keepIISAliveServiceInstaller.ServiceName = "KeepIISAliveService";
            this.keepIISAliveServiceInstaller.ServicesDependedOn = new string[] {
        "W3SVC"};
            this.keepIISAliveServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.keepIISAliveServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.keepIISAliveServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller keepIISAliveServiceInstaller;
    }
}