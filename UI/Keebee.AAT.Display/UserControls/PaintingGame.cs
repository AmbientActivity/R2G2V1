using Keebee.AAT.SystemEventLogging;
using AxShockwaveFlashObjects;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Keebee.AAT.Shared;

namespace Keebee.AAT.Display.UserControls
{
    public partial class PaintingGame : UserControl
    {
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // event handler
        public event EventHandler PaintingGameTimeoutExpiredEvent;

        // delegate
        private delegate void RaisePaintingGameTimeoutExpiredDelegate();

        private bool _enableGameTimeout;

        public PaintingGame()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;
        }

        public void Play(bool enableTimeout)
        {
            _enableGameTimeout = enableTimeout;
            PlayGame();
        }

        private void PlayGame()
        {
            try
            {
                //var enableTimeout = _enableGameTimeout ? 1 : 0;
                //var swf = Path.Combine(Application.StartupPath, "PaintingGame.swf");
                //axShockwaveFlash1.LoadMovie(0, swf);

                //axShockwaveFlash1.CallFunction(
                //    "<invoke name=\"setParameters\"><arguments>" +
                //    $"<number>{enableTimeout}</number></arguments></invoke>");
                //axShockwaveFlash1.CallFunction("<invoke name=\"playPaintingGame\"></invoke>");

                //axShockwaveFlash1.Show();
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.PaintingGame.PlayGame{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop(bool isTimeoutExpired)
        {
            try
            {
                axShockwaveFlash1.Stop();
                axShockwaveFlash1.Hide();

                if (!isTimeoutExpired)
                    axShockwaveFlash1.CallFunction("<invoke name=\"stopPaintingGame\"></invoke>");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.PaintingGame.Stop{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }

        }

        // called by the shockwave activex component
        private void FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            try
            {
                RaisePaintingGameTimeoutExpired();
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.PaintingGame.FlashCall{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void RaisePaintingGameTimeoutExpired()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaisePaintingGameTimeoutExpiredDelegate(RaisePaintingGameTimeoutExpired));
            }
            else
            {
                PaintingGameTimeoutExpiredEvent?.Invoke(new object(), new EventArgs());
            }
        }
    }
}
