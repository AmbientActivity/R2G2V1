namespace Keebee.AAT.StateMachineService
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
            this.stateMachineServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.stateMachineServiceInstaller});
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // stateMachineServiceInstaller
            // 
            this.stateMachineServiceInstaller.DelayedAutoStart = true;
            this.stateMachineServiceInstaller.Description = "Processes all Keebee AAT activities and hands them to the main Display App";
            this.stateMachineServiceInstaller.DisplayName = "Keebee AAT State Machine Service";
            this.stateMachineServiceInstaller.ServiceName = "StateMachineService";
            this.stateMachineServiceInstaller.ServicesDependedOn = new string[] {
        "MSMQ"};
            this.stateMachineServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.stateMachineServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.stateMachineServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller stateMachineServiceInstaller;
    }
}