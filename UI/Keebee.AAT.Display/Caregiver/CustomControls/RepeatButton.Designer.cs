namespace Keebee.AAT.Display.Caregiver.CustomControls
{
    partial class RepeatButton
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
            timerRepeater.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerRepeater = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerRepeater
            // 
            this.timerRepeater.Tick += new System.EventHandler(this.RepeaterTick);
            // 
            // RepeatButton
            // 
            this.Size = new System.Drawing.Size(91, 21);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
