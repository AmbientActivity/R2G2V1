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
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.grpRFReader = new System.Windows.Forms.GroupBox();
            this.grpAutoProfile = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radOn = new System.Windows.Forms.RadioButton();
            this.grpAutoSensor = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSensor = new System.Windows.Forms.Label();
            this.lblProfile = new System.Windows.Forms.Label();
            this.lblSensorLabel = new System.Windows.Forms.Label();
            this.lblProfileLabel = new System.Windows.Forms.Label();
            this.MatchingGameSensor = new System.Windows.Forms.Button();
            this.SlideShowSensor = new System.Windows.Forms.Button();
            this.RadioSensorRight = new System.Windows.Forms.Button();
            this.KillDisplaySensor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.grpInterfaceKit = new System.Windows.Forms.GroupBox();
            this.TelevisionInput = new System.Windows.Forms.Button();
            this.RadioInput = new System.Windows.Forms.Button();
            this.MatchingGameInput = new System.Windows.Forms.Button();
            this.SlideShowInput = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CaregiverButton6 = new System.Windows.Forms.Button();
            this.AmbientButton7 = new System.Windows.Forms.Button();
            this.CatsSensor = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.TelevisionSensorRight = new System.Windows.Forms.Button();
            this.TelevisionSensorLeft = new System.Windows.Forms.Button();
            this.RadioSensorLeft = new System.Windows.Forms.Button();
            this.ReloadConfig = new System.Windows.Forms.Button();
            this.grpRFReader.SuspendLayout();
            this.grpAutoProfile.SuspendLayout();
            this.grpAutoSensor.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpInterfaceKit.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboProfile
            // 
            this.cboProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboProfile.FormattingEnabled = true;
            this.cboProfile.Location = new System.Drawing.Point(9, 30);
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(160, 24);
            this.cboProfile.TabIndex = 5;
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.Location = new System.Drawing.Point(176, 24);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 34);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Activate";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.ActivateProfileClick);
            // 
            // grpRFReader
            // 
            this.grpRFReader.Controls.Add(this.cboProfile);
            this.grpRFReader.Controls.Add(this.btnScan);
            this.grpRFReader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpRFReader.Location = new System.Drawing.Point(13, 57);
            this.grpRFReader.Name = "grpRFReader";
            this.grpRFReader.Size = new System.Drawing.Size(263, 80);
            this.grpRFReader.TabIndex = 7;
            this.grpRFReader.TabStop = false;
            this.grpRFReader.Text = "Profile";
            // 
            // grpAutoProfile
            // 
            this.grpAutoProfile.Controls.Add(this.radioButton1);
            this.grpAutoProfile.Controls.Add(this.radOn);
            this.grpAutoProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAutoProfile.Location = new System.Drawing.Point(282, 67);
            this.grpAutoProfile.Name = "grpAutoProfile";
            this.grpAutoProfile.Size = new System.Drawing.Size(89, 70);
            this.grpAutoProfile.TabIndex = 16;
            this.grpAutoProfile.TabStop = false;
            this.grpAutoProfile.Text = "Auto Profile";
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
            this.radioButton1.CheckedChanged += new System.EventHandler(this.AutoProfileCheckChanged);
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
            this.radOn.CheckedChanged += new System.EventHandler(this.AutoProfileCheckChanged);
            // 
            // grpAutoSensor
            // 
            this.grpAutoSensor.Controls.Add(this.radioButton2);
            this.grpAutoSensor.Controls.Add(this.radioButton3);
            this.grpAutoSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAutoSensor.Location = new System.Drawing.Point(377, 67);
            this.grpAutoSensor.Name = "grpAutoSensor";
            this.grpAutoSensor.Size = new System.Drawing.Size(91, 71);
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
            this.radioButton2.CheckedChanged += new System.EventHandler(this.AutoSensorCheckChanged);
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
            this.radioButton3.CheckedChanged += new System.EventHandler(this.AutoSensorCheckChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblValue);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lblSensor);
            this.groupBox2.Controls.Add(this.lblProfile);
            this.groupBox2.Controls.Add(this.lblSensorLabel);
            this.groupBox2.Controls.Add(this.lblProfileLabel);
            this.groupBox2.Location = new System.Drawing.Point(12, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 46);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.ForeColor = System.Drawing.Color.Red;
            this.lblValue.Location = new System.Drawing.Point(405, 12);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(21, 24);
            this.lblValue.TabIndex = 25;
            this.lblValue.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(346, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 24);
            this.label2.TabIndex = 24;
            this.label2.Text = "Value: ";
            // 
            // lblSensor
            // 
            this.lblSensor.AutoSize = true;
            this.lblSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSensor.ForeColor = System.Drawing.Color.Green;
            this.lblSensor.Location = new System.Drawing.Point(311, 12);
            this.lblSensor.Name = "lblSensor";
            this.lblSensor.Size = new System.Drawing.Size(21, 24);
            this.lblSensor.TabIndex = 23;
            this.lblSensor.Text = "0";
            // 
            // lblProfile
            // 
            this.lblProfile.AutoSize = true;
            this.lblProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProfile.ForeColor = System.Drawing.Color.Blue;
            this.lblProfile.Location = new System.Drawing.Point(70, 12);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(84, 24);
            this.lblProfile.TabIndex = 22;
            this.lblProfile.Text = "Generic";
            // 
            // lblSensorLabel
            // 
            this.lblSensorLabel.AutoSize = true;
            this.lblSensorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSensorLabel.ForeColor = System.Drawing.Color.Green;
            this.lblSensorLabel.Location = new System.Drawing.Point(240, 11);
            this.lblSensorLabel.Name = "lblSensorLabel";
            this.lblSensorLabel.Size = new System.Drawing.Size(80, 24);
            this.lblSensorLabel.TabIndex = 21;
            this.lblSensorLabel.Text = "Sensor: ";
            // 
            // lblProfileLabel
            // 
            this.lblProfileLabel.AutoSize = true;
            this.lblProfileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProfileLabel.ForeColor = System.Drawing.Color.Blue;
            this.lblProfileLabel.Location = new System.Drawing.Point(8, 11);
            this.lblProfileLabel.Name = "lblProfileLabel";
            this.lblProfileLabel.Size = new System.Drawing.Size(67, 24);
            this.lblProfileLabel.TabIndex = 20;
            this.lblProfileLabel.Text = "Profile:";
            // 
            // MatchingGameSensor
            // 
            this.MatchingGameSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MatchingGameSensor.Location = new System.Drawing.Point(117, 63);
            this.MatchingGameSensor.Name = "MatchingGameSensor";
            this.MatchingGameSensor.Size = new System.Drawing.Size(105, 36);
            this.MatchingGameSensor.TabIndex = 2;
            this.MatchingGameSensor.Text = "Matching Game";
            this.MatchingGameSensor.UseMnemonic = false;
            this.MatchingGameSensor.UseVisualStyleBackColor = true;
            this.MatchingGameSensor.Click += new System.EventHandler(this.MatchinggameSensorClick);
            // 
            // SlideShowSensor
            // 
            this.SlideShowSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlideShowSensor.Location = new System.Drawing.Point(6, 63);
            this.SlideShowSensor.Name = "SlideShowSensor";
            this.SlideShowSensor.Size = new System.Drawing.Size(105, 36);
            this.SlideShowSensor.TabIndex = 1;
            this.SlideShowSensor.Text = "Slide Show";
            this.SlideShowSensor.UseMnemonic = false;
            this.SlideShowSensor.UseVisualStyleBackColor = true;
            this.SlideShowSensor.Click += new System.EventHandler(this.SlideShowSensorClick);
            // 
            // RadioSensorRight
            // 
            this.RadioSensorRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RadioSensorRight.Location = new System.Drawing.Point(55, 132);
            this.RadioSensorRight.Name = "RadioSensorRight";
            this.RadioSensorRight.Size = new System.Drawing.Size(49, 30);
            this.RadioSensorRight.TabIndex = 4;
            this.RadioSensorRight.Text = ">";
            this.RadioSensorRight.UseMnemonic = false;
            this.RadioSensorRight.UseVisualStyleBackColor = true;
            this.RadioSensorRight.Click += new System.EventHandler(this.RadioSensorRightClick);
            // 
            // KillDisplaySensor
            // 
            this.KillDisplaySensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KillDisplaySensor.Location = new System.Drawing.Point(339, 63);
            this.KillDisplaySensor.Name = "KillDisplaySensor";
            this.KillDisplaySensor.Size = new System.Drawing.Size(105, 36);
            this.KillDisplaySensor.TabIndex = 15;
            this.KillDisplaySensor.Text = "Kill Display (3)";
            this.KillDisplaySensor.UseVisualStyleBackColor = true;
            this.KillDisplaySensor.Click += new System.EventHandler(this.KillDisplaySensorClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(28, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Radio";
            // 
            // grpInterfaceKit
            // 
            this.grpInterfaceKit.Controls.Add(this.ReloadConfig);
            this.grpInterfaceKit.Controls.Add(this.TelevisionInput);
            this.grpInterfaceKit.Controls.Add(this.RadioInput);
            this.grpInterfaceKit.Controls.Add(this.MatchingGameInput);
            this.grpInterfaceKit.Controls.Add(this.SlideShowInput);
            this.grpInterfaceKit.Controls.Add(this.label3);
            this.grpInterfaceKit.Controls.Add(this.label1);
            this.grpInterfaceKit.Controls.Add(this.CaregiverButton6);
            this.grpInterfaceKit.Controls.Add(this.AmbientButton7);
            this.grpInterfaceKit.Controls.Add(this.CatsSensor);
            this.grpInterfaceKit.Controls.Add(this.label6);
            this.grpInterfaceKit.Controls.Add(this.TelevisionSensorRight);
            this.grpInterfaceKit.Controls.Add(this.TelevisionSensorLeft);
            this.grpInterfaceKit.Controls.Add(this.label5);
            this.grpInterfaceKit.Controls.Add(this.KillDisplaySensor);
            this.grpInterfaceKit.Controls.Add(this.RadioSensorRight);
            this.grpInterfaceKit.Controls.Add(this.SlideShowSensor);
            this.grpInterfaceKit.Controls.Add(this.RadioSensorLeft);
            this.grpInterfaceKit.Controls.Add(this.MatchingGameSensor);
            this.grpInterfaceKit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpInterfaceKit.Location = new System.Drawing.Point(12, 153);
            this.grpInterfaceKit.Name = "grpInterfaceKit";
            this.grpInterfaceKit.Size = new System.Drawing.Size(456, 247);
            this.grpInterfaceKit.TabIndex = 10;
            this.grpInterfaceKit.TabStop = false;
            this.grpInterfaceKit.Text = "Interface Kit";
            // 
            // TelevisionInput
            // 
            this.TelevisionInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TelevisionInput.Location = new System.Drawing.Point(339, 200);
            this.TelevisionInput.Name = "TelevisionInput";
            this.TelevisionInput.Size = new System.Drawing.Size(105, 36);
            this.TelevisionInput.TabIndex = 36;
            this.TelevisionInput.Text = "Television";
            this.TelevisionInput.UseVisualStyleBackColor = true;
            this.TelevisionInput.Click += new System.EventHandler(this.TelevisionInputClick);
            // 
            // RadioInput
            // 
            this.RadioInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RadioInput.Location = new System.Drawing.Point(228, 200);
            this.RadioInput.Name = "RadioInput";
            this.RadioInput.Size = new System.Drawing.Size(105, 36);
            this.RadioInput.TabIndex = 35;
            this.RadioInput.Text = "Radio";
            this.RadioInput.UseVisualStyleBackColor = true;
            this.RadioInput.Click += new System.EventHandler(this.RadioInputClick);
            // 
            // MatchingGameInput
            // 
            this.MatchingGameInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MatchingGameInput.Location = new System.Drawing.Point(117, 200);
            this.MatchingGameInput.Name = "MatchingGameInput";
            this.MatchingGameInput.Size = new System.Drawing.Size(105, 36);
            this.MatchingGameInput.TabIndex = 34;
            this.MatchingGameInput.Text = "Matching Game";
            this.MatchingGameInput.UseMnemonic = false;
            this.MatchingGameInput.UseVisualStyleBackColor = true;
            this.MatchingGameInput.Click += new System.EventHandler(this.MatchingGameInputClick);
            // 
            // SlideShowInput
            // 
            this.SlideShowInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlideShowInput.Location = new System.Drawing.Point(6, 200);
            this.SlideShowInput.Name = "SlideShowInput";
            this.SlideShowInput.Size = new System.Drawing.Size(105, 36);
            this.SlideShowInput.TabIndex = 33;
            this.SlideShowInput.Text = "Slide Show";
            this.SlideShowInput.UseMnemonic = false;
            this.SlideShowInput.UseVisualStyleBackColor = true;
            this.SlideShowInput.Click += new System.EventHandler(this.SlideShowInputClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 32;
            this.label3.Text = "Inputs";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 31;
            this.label1.Text = "Sensors";
            // 
            // CaregiverButton6
            // 
            this.CaregiverButton6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CaregiverButton6.Location = new System.Drawing.Point(228, 126);
            this.CaregiverButton6.Name = "CaregiverButton6";
            this.CaregiverButton6.Size = new System.Drawing.Size(105, 36);
            this.CaregiverButton6.TabIndex = 30;
            this.CaregiverButton6.Text = "Caregiver (6)";
            this.CaregiverButton6.UseVisualStyleBackColor = true;
            this.CaregiverButton6.Click += new System.EventHandler(this.CaregiverSensorClick);
            // 
            // AmbientButton7
            // 
            this.AmbientButton7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AmbientButton7.Location = new System.Drawing.Point(339, 126);
            this.AmbientButton7.Name = "AmbientButton7";
            this.AmbientButton7.Size = new System.Drawing.Size(105, 36);
            this.AmbientButton7.TabIndex = 29;
            this.AmbientButton7.Text = "Ambient (7)";
            this.AmbientButton7.UseVisualStyleBackColor = true;
            this.AmbientButton7.Click += new System.EventHandler(this.AmbientSensorClick);
            // 
            // CatsSensor
            // 
            this.CatsSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CatsSensor.Location = new System.Drawing.Point(228, 63);
            this.CatsSensor.Name = "CatsSensor";
            this.CatsSensor.Size = new System.Drawing.Size(105, 36);
            this.CatsSensor.TabIndex = 28;
            this.CatsSensor.Text = "Cats";
            this.CatsSensor.UseVisualStyleBackColor = true;
            this.CatsSensor.Click += new System.EventHandler(this.CatsSensorClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(130, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 15);
            this.label6.TabIndex = 27;
            this.label6.Text = "Television";
            // 
            // TelevisionSensorRight
            // 
            this.TelevisionSensorRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TelevisionSensorRight.Location = new System.Drawing.Point(169, 132);
            this.TelevisionSensorRight.Name = "TelevisionSensorRight";
            this.TelevisionSensorRight.Size = new System.Drawing.Size(49, 30);
            this.TelevisionSensorRight.TabIndex = 25;
            this.TelevisionSensorRight.Text = ">";
            this.TelevisionSensorRight.UseMnemonic = false;
            this.TelevisionSensorRight.UseVisualStyleBackColor = true;
            this.TelevisionSensorRight.MouseCaptureChanged += new System.EventHandler(this.TelevisionSensorRightClick);
            // 
            // TelevisionSensorLeft
            // 
            this.TelevisionSensorLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TelevisionSensorLeft.Location = new System.Drawing.Point(119, 132);
            this.TelevisionSensorLeft.Name = "TelevisionSensorLeft";
            this.TelevisionSensorLeft.Size = new System.Drawing.Size(49, 30);
            this.TelevisionSensorLeft.TabIndex = 26;
            this.TelevisionSensorLeft.Text = "<";
            this.TelevisionSensorLeft.UseMnemonic = false;
            this.TelevisionSensorLeft.UseVisualStyleBackColor = true;
            this.TelevisionSensorLeft.Click += new System.EventHandler(this.TelevisionSensorLeftClick);
            // 
            // RadioSensorLeft
            // 
            this.RadioSensorLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RadioSensorLeft.Location = new System.Drawing.Point(5, 132);
            this.RadioSensorLeft.Name = "RadioSensorLeft";
            this.RadioSensorLeft.Size = new System.Drawing.Size(49, 30);
            this.RadioSensorLeft.TabIndex = 4;
            this.RadioSensorLeft.Text = "<";
            this.RadioSensorLeft.UseMnemonic = false;
            this.RadioSensorLeft.UseVisualStyleBackColor = true;
            this.RadioSensorLeft.Click += new System.EventHandler(this.RadioSensorLeftClick);
            // 
            // ReloadConfig
            // 
            this.ReloadConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReloadConfig.Location = new System.Drawing.Point(339, 20);
            this.ReloadConfig.Name = "ReloadConfig";
            this.ReloadConfig.Size = new System.Drawing.Size(105, 36);
            this.ReloadConfig.TabIndex = 37;
            this.ReloadConfig.Text = "Reload Config";
            this.ReloadConfig.UseMnemonic = false;
            this.ReloadConfig.UseVisualStyleBackColor = true;
            this.ReloadConfig.Click += new System.EventHandler(this.ReloadConfigClick);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 409);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpAutoSensor);
            this.Controls.Add(this.grpAutoProfile);
            this.Controls.Add(this.grpInterfaceKit);
            this.Controls.Add(this.grpRFReader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlPanel";
            this.ShowInTaskbar = false;
            this.Text = "Keebee AAT Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanelClosing);
            this.grpRFReader.ResumeLayout(false);
            this.grpAutoProfile.ResumeLayout(false);
            this.grpAutoProfile.PerformLayout();
            this.grpAutoSensor.ResumeLayout(false);
            this.grpAutoSensor.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grpInterfaceKit.ResumeLayout(false);
            this.grpInterfaceKit.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.GroupBox grpRFReader;
        private System.Windows.Forms.GroupBox grpAutoProfile;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radOn;
        private System.Windows.Forms.GroupBox grpAutoSensor;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblSensor;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.Label lblSensorLabel;
        private System.Windows.Forms.Label lblProfileLabel;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button MatchingGameSensor;
        private System.Windows.Forms.Button SlideShowSensor;
        private System.Windows.Forms.Button RadioSensorRight;
        private System.Windows.Forms.Button KillDisplaySensor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpInterfaceKit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button TelevisionSensorRight;
        private System.Windows.Forms.Button TelevisionSensorLeft;
        private System.Windows.Forms.Button RadioSensorLeft;
        private System.Windows.Forms.Button CaregiverButton6;
        private System.Windows.Forms.Button AmbientButton7;
        private System.Windows.Forms.Button CatsSensor;
        private System.Windows.Forms.Button TelevisionInput;
        private System.Windows.Forms.Button RadioInput;
        private System.Windows.Forms.Button MatchingGameInput;
        private System.Windows.Forms.Button SlideShowInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ReloadConfig;
    }
}

