namespace Keebee.AAT.VideoCaptureService
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
            this.videoCaptureServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.serviceProcessInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.videoCaptureServiceInstaller});
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // videoCaptureServiceInstaller
            // 
            this.videoCaptureServiceInstaller.Description = "Does video capturing of users that interact with R2G2";
            this.videoCaptureServiceInstaller.DisplayName = "Keebee AAT Video Capture Service";
            this.videoCaptureServiceInstaller.ServiceName = "VideoCaptureService";
            this.videoCaptureServiceInstaller.ServicesDependedOn = new string[] {
        "MSMQ",
        "StateMachineService"};
            this.videoCaptureServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.videoCaptureServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.videoCaptureServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller videoCaptureServiceInstaller;
    }
}