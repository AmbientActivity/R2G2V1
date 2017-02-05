namespace Keebee.AAT.Simulator
{
    partial class ControlPanel
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboResident = new System.Windows.Forms.ComboBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.grpMediaSource = new System.Windows.Forms.GroupBox();
            this.grpAutoResident = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radOn = new System.Windows.Forms.RadioButton();
            this.grpAutoSensor = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.btnMatchingGame = new System.Windows.Forms.Button();
            this.btnSlideShow = new System.Windows.Forms.Button();
            this.bnRadioRight = new System.Windows.Forms.Button();
            this.btnKillDisplay = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.grpInterfaceKit = new System.Windows.Forms.GroupBox();
            this.btnOffScreen = new System.Windows.Forms.Button();
            this.btnCaregiver = new System.Windows.Forms.Button();
            this.btnAmbient = new System.Windows.Forms.Button();
            this.btnCats = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnTelevisionRight = new System.Windows.Forms.Button();
            this.btnTelevisionLeft = new System.Windows.Forms.Button();
            this.bnRadioLeft = new System.Windows.Forms.Button();
            this.btnPaintingActivity = new System.Windows.Forms.Button();
            this.grpMediaSource.SuspendLayout();
            this.grpAutoResident.SuspendLayout();
            this.grpAutoSensor.SuspendLayout();
            this.grpInterfaceKit.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboResident
            // 
            this.cboResident.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboResident.FormattingEnabled = true;
            this.cboResident.Location = new System.Drawing.Point(9, 26);
            this.cboResident.Name = "cboResident";
            this.cboResident.Size = new System.Drawing.Size(257, 24);
            this.cboResident.TabIndex = 5;
            this.cboResident.SelectedIndexChanged += new System.EventHandler(this.ResidentSelectedIndexChanged);
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.Location = new System.Drawing.Point(283, 20);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(127, 34);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Activate";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.ActivateResidentClick);
            // 
            // grpMediaSource
            // 
            this.grpMediaSource.Controls.Add(this.cboResident);
            this.grpMediaSource.Controls.Add(this.btnScan);
            this.grpMediaSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMediaSource.Location = new System.Drawing.Point(13, 17);
            this.grpMediaSource.Name = "grpMediaSource";
            this.grpMediaSource.Size = new System.Drawing.Size(420, 67);
            this.grpMediaSource.TabIndex = 7;
            this.grpMediaSource.TabStop = false;
            this.grpMediaSource.Text = "Media Source";
            // 
            // grpAutoResident
            // 
            this.grpAutoResident.Controls.Add(this.radioButton1);
            this.grpAutoResident.Controls.Add(this.radOn);
            this.grpAutoResident.Enabled = false;
            this.grpAutoResident.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAutoResident.Location = new System.Drawing.Point(15, 92);
            this.grpAutoResident.Name = "grpAutoResident";
            this.grpAutoResident.Size = new System.Drawing.Size(124, 70);
            this.grpAutoResident.TabIndex = 16;
            this.grpAutoResident.TabStop = false;
            this.grpAutoResident.Text = "Auto Resident";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(11, 40);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(39, 17);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Off";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.AutoResidentCheckChanged);
            // 
            // radOn
            // 
            this.radOn.AutoSize = true;
            this.radOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radOn.Location = new System.Drawing.Point(11, 19);
            this.radOn.Name = "radOn";
            this.radOn.Size = new System.Drawing.Size(39, 17);
            this.radOn.TabIndex = 0;
            this.radOn.TabStop = true;
            this.radOn.Text = "On";
            this.radOn.UseVisualStyleBackColor = true;
            this.radOn.CheckedChanged += new System.EventHandler(this.AutoResidentCheckChanged);
            // 
            // grpAutoSensor
            // 
            this.grpAutoSensor.Controls.Add(this.radioButton2);
            this.grpAutoSensor.Controls.Add(this.radioButton3);
            this.grpAutoSensor.Enabled = false;
            this.grpAutoSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAutoSensor.Location = new System.Drawing.Point(158, 92);
            this.grpAutoSensor.Name = "grpAutoSensor";
            this.grpAutoSensor.Size = new System.Drawing.Size(121, 71);
            this.grpAutoSensor.TabIndex = 17;
            this.grpAutoSensor.TabStop = false;
            this.grpAutoSensor.Text = "Auto Sensor";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.Location = new System.Drawing.Point(11, 40);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(39, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Off";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.AutoResponseCheckChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton3.Location = new System.Drawing.Point(11, 19);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(39, 17);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.Text = "On";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.AutoResponseCheckChanged);
            // 
            // btnMatchingGame
            // 
            this.btnMatchingGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMatchingGame.Location = new System.Drawing.Point(112, 28);
            this.btnMatchingGame.Name = "btnMatchingGame";
            this.btnMatchingGame.Size = new System.Drawing.Size(96, 36);
            this.btnMatchingGame.TabIndex = 2;
            this.btnMatchingGame.Text = "Matching Game";
            this.btnMatchingGame.UseMnemonic = false;
            this.btnMatchingGame.UseVisualStyleBackColor = true;
            this.btnMatchingGame.Click += new System.EventHandler(this.MatchingGameButtonClick);
            // 
            // btnSlideShow
            // 
            this.btnSlideShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSlideShow.Location = new System.Drawing.Point(10, 28);
            this.btnSlideShow.Name = "btnSlideShow";
            this.btnSlideShow.Size = new System.Drawing.Size(96, 36);
            this.btnSlideShow.TabIndex = 1;
            this.btnSlideShow.Text = "Slide Show";
            this.btnSlideShow.UseMnemonic = false;
            this.btnSlideShow.UseVisualStyleBackColor = true;
            this.btnSlideShow.Click += new System.EventHandler(this.SlideShowButtonClick);
            // 
            // bnRadioRight
            // 
            this.bnRadioRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnRadioRight.Location = new System.Drawing.Point(58, 91);
            this.bnRadioRight.Name = "bnRadioRight";
            this.bnRadioRight.Size = new System.Drawing.Size(48, 36);
            this.bnRadioRight.TabIndex = 4;
            this.bnRadioRight.Text = ">";
            this.bnRadioRight.UseMnemonic = false;
            this.bnRadioRight.UseVisualStyleBackColor = true;
            this.bnRadioRight.Click += new System.EventHandler(this.RadioRightButtonClick);
            // 
            // btnKillDisplay
            // 
            this.btnKillDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKillDisplay.Location = new System.Drawing.Point(296, 97);
            this.btnKillDisplay.Name = "btnKillDisplay";
            this.btnKillDisplay.Size = new System.Drawing.Size(127, 65);
            this.btnKillDisplay.TabIndex = 15;
            this.btnKillDisplay.Text = "Kill Display";
            this.btnKillDisplay.UseVisualStyleBackColor = true;
            this.btnKillDisplay.Click += new System.EventHandler(this.KillDisplayButtonClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(39, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Radio";
            // 
            // grpInterfaceKit
            // 
            this.grpInterfaceKit.Controls.Add(this.btnPaintingActivity);
            this.grpInterfaceKit.Controls.Add(this.btnOffScreen);
            this.grpInterfaceKit.Controls.Add(this.btnCaregiver);
            this.grpInterfaceKit.Controls.Add(this.btnAmbient);
            this.grpInterfaceKit.Controls.Add(this.btnCats);
            this.grpInterfaceKit.Controls.Add(this.label6);
            this.grpInterfaceKit.Controls.Add(this.btnTelevisionRight);
            this.grpInterfaceKit.Controls.Add(this.btnTelevisionLeft);
            this.grpInterfaceKit.Controls.Add(this.label5);
            this.grpInterfaceKit.Controls.Add(this.bnRadioRight);
            this.grpInterfaceKit.Controls.Add(this.btnSlideShow);
            this.grpInterfaceKit.Controls.Add(this.bnRadioLeft);
            this.grpInterfaceKit.Controls.Add(this.btnMatchingGame);
            this.grpInterfaceKit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpInterfaceKit.Location = new System.Drawing.Point(12, 171);
            this.grpInterfaceKit.Name = "grpInterfaceKit";
            this.grpInterfaceKit.Size = new System.Drawing.Size(421, 195);
            this.grpInterfaceKit.TabIndex = 10;
            this.grpInterfaceKit.TabStop = false;
            this.grpInterfaceKit.Text = "Repsonse";
            // 
            // btnOffScreen
            // 
            this.btnOffScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOffScreen.Location = new System.Drawing.Point(315, 28);
            this.btnOffScreen.Name = "btnOffScreen";
            this.btnOffScreen.Size = new System.Drawing.Size(96, 36);
            this.btnOffScreen.TabIndex = 37;
            this.btnOffScreen.Text = "Off Screen";
            this.btnOffScreen.UseVisualStyleBackColor = true;
            this.btnOffScreen.Click += new System.EventHandler(this.OffScreenButtonClick);
            // 
            // btnCaregiver
            // 
            this.btnCaregiver.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCaregiver.Location = new System.Drawing.Point(213, 91);
            this.btnCaregiver.Name = "btnCaregiver";
            this.btnCaregiver.Size = new System.Drawing.Size(96, 36);
            this.btnCaregiver.TabIndex = 30;
            this.btnCaregiver.Text = "Caregiver";
            this.btnCaregiver.UseVisualStyleBackColor = true;
            this.btnCaregiver.Click += new System.EventHandler(this.CaregiverButtonClick);
            // 
            // btnAmbient
            // 
            this.btnAmbient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAmbient.Location = new System.Drawing.Point(315, 91);
            this.btnAmbient.Name = "btnAmbient";
            this.btnAmbient.Size = new System.Drawing.Size(96, 36);
            this.btnAmbient.TabIndex = 29;
            this.btnAmbient.Text = "Ambient";
            this.btnAmbient.UseVisualStyleBackColor = true;
            this.btnAmbient.Click += new System.EventHandler(this.AmbientButtonClick);
            // 
            // btnCats
            // 
            this.btnCats.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCats.Location = new System.Drawing.Point(213, 28);
            this.btnCats.Name = "btnCats";
            this.btnCats.Size = new System.Drawing.Size(96, 36);
            this.btnCats.TabIndex = 28;
            this.btnCats.Text = "Cats";
            this.btnCats.UseVisualStyleBackColor = true;
            this.btnCats.Click += new System.EventHandler(this.CatsButtonClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(130, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 15);
            this.label6.TabIndex = 27;
            this.label6.Text = "Television";
            // 
            // btnTelevisionRight
            // 
            this.btnTelevisionRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTelevisionRight.Location = new System.Drawing.Point(160, 91);
            this.btnTelevisionRight.Name = "btnTelevisionRight";
            this.btnTelevisionRight.Size = new System.Drawing.Size(48, 36);
            this.btnTelevisionRight.TabIndex = 25;
            this.btnTelevisionRight.Text = ">";
            this.btnTelevisionRight.UseMnemonic = false;
            this.btnTelevisionRight.UseVisualStyleBackColor = true;
            this.btnTelevisionRight.MouseCaptureChanged += new System.EventHandler(this.TelevisionRightButtonClick);
            // 
            // btnTelevisionLeft
            // 
            this.btnTelevisionLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTelevisionLeft.Location = new System.Drawing.Point(112, 91);
            this.btnTelevisionLeft.Name = "btnTelevisionLeft";
            this.btnTelevisionLeft.Size = new System.Drawing.Size(48, 36);
            this.btnTelevisionLeft.TabIndex = 26;
            this.btnTelevisionLeft.Text = "<";
            this.btnTelevisionLeft.UseMnemonic = false;
            this.btnTelevisionLeft.UseVisualStyleBackColor = true;
            this.btnTelevisionLeft.Click += new System.EventHandler(this.TelevisionLeftButtonClick);
            // 
            // bnRadioLeft
            // 
            this.bnRadioLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnRadioLeft.Location = new System.Drawing.Point(10, 91);
            this.bnRadioLeft.Name = "bnRadioLeft";
            this.bnRadioLeft.Size = new System.Drawing.Size(48, 36);
            this.bnRadioLeft.TabIndex = 4;
            this.bnRadioLeft.Text = "<";
            this.bnRadioLeft.UseMnemonic = false;
            this.bnRadioLeft.UseVisualStyleBackColor = true;
            this.bnRadioLeft.Click += new System.EventHandler(this.RadioLeftButtonClick);
            // 
            // btnPaintingActivity
            // 
            this.btnPaintingActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPaintingActivity.Location = new System.Drawing.Point(10, 151);
            this.btnPaintingActivity.Name = "btnPaintingActivity";
            this.btnPaintingActivity.Size = new System.Drawing.Size(96, 36);
            this.btnPaintingActivity.TabIndex = 38;
            this.btnPaintingActivity.Text = "Painting Game";
            this.btnPaintingActivity.UseMnemonic = false;
            this.btnPaintingActivity.UseVisualStyleBackColor = true;
            this.btnPaintingActivity.Click += new System.EventHandler(this.PaintingActivityClick);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 372);
            this.Controls.Add(this.grpAutoSensor);
            this.Controls.Add(this.grpAutoResident);
            this.Controls.Add(this.grpInterfaceKit);
            this.Controls.Add(this.grpMediaSource);
            this.Controls.Add(this.btnKillDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ControlPanel";
            this.Text = "Keebee AAT Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanelClosing);
            this.grpMediaSource.ResumeLayout(false);
            this.grpAutoResident.ResumeLayout(false);
            this.grpAutoResident.PerformLayout();
            this.grpAutoSensor.ResumeLayout(false);
            this.grpAutoSensor.PerformLayout();
            this.grpInterfaceKit.ResumeLayout(false);
            this.grpInterfaceKit.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboResident;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.GroupBox grpMediaSource;
        private System.Windows.Forms.GroupBox grpAutoResident;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radOn;
        private System.Windows.Forms.GroupBox grpAutoSensor;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Button btnMatchingGame;
        private System.Windows.Forms.Button btnSlideShow;
        private System.Windows.Forms.Button bnRadioRight;
        private System.Windows.Forms.Button btnKillDisplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpInterfaceKit;
        private System.Windows.Forms.Button btnTelevisionRight;
        private System.Windows.Forms.Button btnTelevisionLeft;
        private System.Windows.Forms.Button bnRadioLeft;
        private System.Windows.Forms.Button btnCaregiver;
        private System.Windows.Forms.Button btnAmbient;
        private System.Windows.Forms.Button btnCats;
        private System.Windows.Forms.Button btnOffScreen;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnPaintingActivity;
    }
}

