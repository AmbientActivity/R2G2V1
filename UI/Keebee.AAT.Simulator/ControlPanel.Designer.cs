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
            this.btnKillDisplay = new System.Windows.Forms.Button();
            this.btnMatchingGame = new System.Windows.Forms.Button();
            this.btnSlideShow = new System.Windows.Forms.Button();
            this.bnRadioRight = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.grpInterfaceKit = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCuteRight = new System.Windows.Forms.Button();
            this.btnCuteLeft = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAnimalsRight = new System.Windows.Forms.Button();
            this.btnAnimalsLeft = new System.Windows.Forms.Button();
            this.btnMachineryRight = new System.Windows.Forms.Button();
            this.btnMachineryLeft = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSportsRight = new System.Windows.Forms.Button();
            this.btnSportsLeft = new System.Windows.Forms.Button();
            this.btnNatureRight = new System.Windows.Forms.Button();
            this.btnNatureLeft = new System.Windows.Forms.Button();
            this.btnBalloonPoppingGame = new System.Windows.Forms.Button();
            this.btnVolumeControl = new System.Windows.Forms.Button();
            this.btnPaintingActivity = new System.Windows.Forms.Button();
            this.btnOffScreen = new System.Windows.Forms.Button();
            this.btnCaregiver = new System.Windows.Forms.Button();
            this.btnAmbient = new System.Windows.Forms.Button();
            this.btnCats = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnTelevisionRight = new System.Windows.Forms.Button();
            this.btnTelevisionLeft = new System.Windows.Forms.Button();
            this.bnRadioLeft = new System.Windows.Forms.Button();
            this.btnRandom = new System.Windows.Forms.Button();
            this.grpMediaSource.SuspendLayout();
            this.grpInterfaceKit.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboResident
            // 
            this.cboResident.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboResident.FormattingEnabled = true;
            this.cboResident.Location = new System.Drawing.Point(9, 26);
            this.cboResident.Name = "cboResident";
            this.cboResident.Size = new System.Drawing.Size(219, 24);
            this.cboResident.TabIndex = 5;
            this.cboResident.SelectedIndexChanged += new System.EventHandler(this.ResidentSelectedIndexChanged);
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.Location = new System.Drawing.Point(234, 20);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 35);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Activate";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.ActivateResidentClick);
            // 
            // grpMediaSource
            // 
            this.grpMediaSource.Controls.Add(this.cboResident);
            this.grpMediaSource.Controls.Add(this.btnScan);
            this.grpMediaSource.Controls.Add(this.btnKillDisplay);
            this.grpMediaSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMediaSource.Location = new System.Drawing.Point(13, 17);
            this.grpMediaSource.Name = "grpMediaSource";
            this.grpMediaSource.Size = new System.Drawing.Size(420, 67);
            this.grpMediaSource.TabIndex = 7;
            this.grpMediaSource.TabStop = false;
            this.grpMediaSource.Text = "Media Source";
            // 
            // btnKillDisplay
            // 
            this.btnKillDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKillDisplay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnKillDisplay.Location = new System.Drawing.Point(315, 20);
            this.btnKillDisplay.Name = "btnKillDisplay";
            this.btnKillDisplay.Size = new System.Drawing.Size(94, 35);
            this.btnKillDisplay.TabIndex = 15;
            this.btnKillDisplay.Text = "Kill Display";
            this.btnKillDisplay.UseVisualStyleBackColor = true;
            this.btnKillDisplay.Click += new System.EventHandler(this.KillDisplayButtonClick);
            // 
            // btnMatchingGame
            // 
            this.btnMatchingGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMatchingGame.Location = new System.Drawing.Point(213, 203);
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
            this.btnSlideShow.Location = new System.Drawing.Point(112, 31);
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
            this.grpInterfaceKit.Controls.Add(this.btnRandom);
            this.grpInterfaceKit.Controls.Add(this.label8);
            this.grpInterfaceKit.Controls.Add(this.btnCuteRight);
            this.grpInterfaceKit.Controls.Add(this.btnCuteLeft);
            this.grpInterfaceKit.Controls.Add(this.label3);
            this.grpInterfaceKit.Controls.Add(this.label4);
            this.grpInterfaceKit.Controls.Add(this.btnAnimalsRight);
            this.grpInterfaceKit.Controls.Add(this.btnAnimalsLeft);
            this.grpInterfaceKit.Controls.Add(this.btnMachineryRight);
            this.grpInterfaceKit.Controls.Add(this.btnMachineryLeft);
            this.grpInterfaceKit.Controls.Add(this.label2);
            this.grpInterfaceKit.Controls.Add(this.label1);
            this.grpInterfaceKit.Controls.Add(this.btnSportsRight);
            this.grpInterfaceKit.Controls.Add(this.btnSportsLeft);
            this.grpInterfaceKit.Controls.Add(this.btnNatureRight);
            this.grpInterfaceKit.Controls.Add(this.btnNatureLeft);
            this.grpInterfaceKit.Controls.Add(this.btnBalloonPoppingGame);
            this.grpInterfaceKit.Controls.Add(this.btnVolumeControl);
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
            this.grpInterfaceKit.Location = new System.Drawing.Point(13, 90);
            this.grpInterfaceKit.Name = "grpInterfaceKit";
            this.grpInterfaceKit.Size = new System.Drawing.Size(421, 304);
            this.grpInterfaceKit.TabIndex = 10;
            this.grpInterfaceKit.TabStop = false;
            this.grpInterfaceKit.Text = "Repsonse";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(245, 130);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 15);
            this.label8.TabIndex = 57;
            this.label8.Text = "Cute";
            // 
            // btnCuteRight
            // 
            this.btnCuteRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCuteRight.Location = new System.Drawing.Point(261, 151);
            this.btnCuteRight.Name = "btnCuteRight";
            this.btnCuteRight.Size = new System.Drawing.Size(48, 36);
            this.btnCuteRight.TabIndex = 53;
            this.btnCuteRight.Text = ">";
            this.btnCuteRight.UseMnemonic = false;
            this.btnCuteRight.UseVisualStyleBackColor = true;
            this.btnCuteRight.Click += new System.EventHandler(this.CuteRightButtonClick);
            // 
            // btnCuteLeft
            // 
            this.btnCuteLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCuteLeft.Location = new System.Drawing.Point(213, 151);
            this.btnCuteLeft.Name = "btnCuteLeft";
            this.btnCuteLeft.Size = new System.Drawing.Size(48, 36);
            this.btnCuteLeft.TabIndex = 54;
            this.btnCuteLeft.Text = "<";
            this.btnCuteLeft.UseMnemonic = false;
            this.btnCuteLeft.UseVisualStyleBackColor = true;
            this.btnCuteLeft.Click += new System.EventHandler(this.CuteLeftButtonClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(138, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 52;
            this.label3.Text = "Animals";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(29, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 15);
            this.label4.TabIndex = 51;
            this.label4.Text = "Machinery";
            // 
            // btnAnimalsRight
            // 
            this.btnAnimalsRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnimalsRight.Location = new System.Drawing.Point(160, 151);
            this.btnAnimalsRight.Name = "btnAnimalsRight";
            this.btnAnimalsRight.Size = new System.Drawing.Size(48, 36);
            this.btnAnimalsRight.TabIndex = 49;
            this.btnAnimalsRight.Text = ">";
            this.btnAnimalsRight.UseMnemonic = false;
            this.btnAnimalsRight.UseVisualStyleBackColor = true;
            this.btnAnimalsRight.Click += new System.EventHandler(this.AnimalsRightButtonClick);
            // 
            // btnAnimalsLeft
            // 
            this.btnAnimalsLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnimalsLeft.Location = new System.Drawing.Point(112, 151);
            this.btnAnimalsLeft.Name = "btnAnimalsLeft";
            this.btnAnimalsLeft.Size = new System.Drawing.Size(48, 36);
            this.btnAnimalsLeft.TabIndex = 50;
            this.btnAnimalsLeft.Text = "<";
            this.btnAnimalsLeft.UseMnemonic = false;
            this.btnAnimalsLeft.UseVisualStyleBackColor = true;
            this.btnAnimalsLeft.Click += new System.EventHandler(this.AnimalsLeftButtonClick);
            // 
            // btnMachineryRight
            // 
            this.btnMachineryRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMachineryRight.Location = new System.Drawing.Point(58, 151);
            this.btnMachineryRight.Name = "btnMachineryRight";
            this.btnMachineryRight.Size = new System.Drawing.Size(48, 36);
            this.btnMachineryRight.TabIndex = 47;
            this.btnMachineryRight.Text = ">";
            this.btnMachineryRight.UseMnemonic = false;
            this.btnMachineryRight.UseVisualStyleBackColor = true;
            this.btnMachineryRight.Click += new System.EventHandler(this.MachineryRightButtonClick);
            // 
            // btnMachineryLeft
            // 
            this.btnMachineryLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMachineryLeft.Location = new System.Drawing.Point(10, 151);
            this.btnMachineryLeft.Name = "btnMachineryLeft";
            this.btnMachineryLeft.Size = new System.Drawing.Size(48, 36);
            this.btnMachineryLeft.TabIndex = 48;
            this.btnMachineryLeft.Text = "<";
            this.btnMachineryLeft.UseMnemonic = false;
            this.btnMachineryLeft.UseVisualStyleBackColor = true;
            this.btnMachineryLeft.Click += new System.EventHandler(this.MachineryLeftButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(341, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 46;
            this.label2.Text = "Sports";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(238, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 45;
            this.label1.Text = "Nature";
            // 
            // btnSportsRight
            // 
            this.btnSportsRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSportsRight.Location = new System.Drawing.Point(363, 91);
            this.btnSportsRight.Name = "btnSportsRight";
            this.btnSportsRight.Size = new System.Drawing.Size(48, 36);
            this.btnSportsRight.TabIndex = 43;
            this.btnSportsRight.Text = ">";
            this.btnSportsRight.UseMnemonic = false;
            this.btnSportsRight.UseVisualStyleBackColor = true;
            this.btnSportsRight.Click += new System.EventHandler(this.SportsRightButtonClick);
            // 
            // btnSportsLeft
            // 
            this.btnSportsLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSportsLeft.Location = new System.Drawing.Point(315, 91);
            this.btnSportsLeft.Name = "btnSportsLeft";
            this.btnSportsLeft.Size = new System.Drawing.Size(48, 36);
            this.btnSportsLeft.TabIndex = 44;
            this.btnSportsLeft.Text = "<";
            this.btnSportsLeft.UseMnemonic = false;
            this.btnSportsLeft.UseVisualStyleBackColor = true;
            this.btnSportsLeft.Click += new System.EventHandler(this.SportsLeftButtonClick);
            // 
            // btnNatureRight
            // 
            this.btnNatureRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNatureRight.Location = new System.Drawing.Point(261, 91);
            this.btnNatureRight.Name = "btnNatureRight";
            this.btnNatureRight.Size = new System.Drawing.Size(48, 36);
            this.btnNatureRight.TabIndex = 41;
            this.btnNatureRight.Text = ">";
            this.btnNatureRight.UseMnemonic = false;
            this.btnNatureRight.UseVisualStyleBackColor = true;
            this.btnNatureRight.Click += new System.EventHandler(this.NatureRightButtonClick);
            // 
            // btnNatureLeft
            // 
            this.btnNatureLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNatureLeft.Location = new System.Drawing.Point(213, 91);
            this.btnNatureLeft.Name = "btnNatureLeft";
            this.btnNatureLeft.Size = new System.Drawing.Size(48, 36);
            this.btnNatureLeft.TabIndex = 42;
            this.btnNatureLeft.Text = "<";
            this.btnNatureLeft.UseMnemonic = false;
            this.btnNatureLeft.UseVisualStyleBackColor = true;
            this.btnNatureLeft.Click += new System.EventHandler(this.NatureLeftButtonClick);
            // 
            // btnBalloonPoppingGame
            // 
            this.btnBalloonPoppingGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBalloonPoppingGame.Location = new System.Drawing.Point(112, 203);
            this.btnBalloonPoppingGame.Name = "btnBalloonPoppingGame";
            this.btnBalloonPoppingGame.Size = new System.Drawing.Size(96, 36);
            this.btnBalloonPoppingGame.TabIndex = 40;
            this.btnBalloonPoppingGame.Text = "Balloon Popping";
            this.btnBalloonPoppingGame.UseMnemonic = false;
            this.btnBalloonPoppingGame.UseVisualStyleBackColor = true;
            this.btnBalloonPoppingGame.Click += new System.EventHandler(this.BalloonPoppingGameClick);
            // 
            // btnVolumeControl
            // 
            this.btnVolumeControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolumeControl.Location = new System.Drawing.Point(213, 31);
            this.btnVolumeControl.Name = "btnVolumeControl";
            this.btnVolumeControl.Size = new System.Drawing.Size(96, 36);
            this.btnVolumeControl.TabIndex = 39;
            this.btnVolumeControl.Text = "Volume Control";
            this.btnVolumeControl.UseMnemonic = false;
            this.btnVolumeControl.UseVisualStyleBackColor = true;
            this.btnVolumeControl.Click += new System.EventHandler(this.VolumeControlClick);
            // 
            // btnPaintingActivity
            // 
            this.btnPaintingActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPaintingActivity.Location = new System.Drawing.Point(10, 203);
            this.btnPaintingActivity.Name = "btnPaintingActivity";
            this.btnPaintingActivity.Size = new System.Drawing.Size(96, 36);
            this.btnPaintingActivity.TabIndex = 38;
            this.btnPaintingActivity.Text = "Painting Activity";
            this.btnPaintingActivity.UseMnemonic = false;
            this.btnPaintingActivity.UseVisualStyleBackColor = true;
            this.btnPaintingActivity.Click += new System.EventHandler(this.PaintingActivityClick);
            // 
            // btnOffScreen
            // 
            this.btnOffScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOffScreen.Location = new System.Drawing.Point(315, 31);
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
            this.btnCaregiver.Location = new System.Drawing.Point(315, 203);
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
            this.btnAmbient.Location = new System.Drawing.Point(10, 31);
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
            this.btnCats.Location = new System.Drawing.Point(315, 151);
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
            // btnRandom
            // 
            this.btnRandom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRandom.Location = new System.Drawing.Point(10, 254);
            this.btnRandom.Name = "btnRandom";
            this.btnRandom.Size = new System.Drawing.Size(96, 36);
            this.btnRandom.TabIndex = 58;
            this.btnRandom.Text = "Random";
            this.btnRandom.UseVisualStyleBackColor = true;
            this.btnRandom.Click += new System.EventHandler(this.RandomButtonClick);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 406);
            this.Controls.Add(this.grpInterfaceKit);
            this.Controls.Add(this.grpMediaSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ControlPanel";
            this.Text = "Keebee AAT Simulator";
            this.grpMediaSource.ResumeLayout(false);
            this.grpInterfaceKit.ResumeLayout(false);
            this.grpInterfaceKit.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboResident;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.GroupBox grpMediaSource;
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
        private System.Windows.Forms.Button btnVolumeControl;
        private System.Windows.Forms.Button btnBalloonPoppingGame;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSportsRight;
        private System.Windows.Forms.Button btnSportsLeft;
        private System.Windows.Forms.Button btnNatureRight;
        private System.Windows.Forms.Button btnNatureLeft;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCuteRight;
        private System.Windows.Forms.Button btnCuteLeft;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAnimalsRight;
        private System.Windows.Forms.Button btnAnimalsLeft;
        private System.Windows.Forms.Button btnMachineryRight;
        private System.Windows.Forms.Button btnMachineryLeft;
        private System.Windows.Forms.Button btnRandom;
    }
}

