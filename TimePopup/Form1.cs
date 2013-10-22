using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimePopup.Properties;

namespace TimePopup
{
    public partial class Form1 : Form
    {
        TimeSpan popupTime = new TimeSpan(16,30,0);
        DateTime nextPopupTime;

        bool showBalloon = true;
        bool showMessageBox = false;

        string timesheetUrl = "http://time";

        bool realExit = false;
        bool startMinimized = false;

        bool logEvents = true;
        string logPath = "C:\\TimePopup.log";

        DateTime? firstUnlock = null;
        DateTime appStart = DateTime.Now;

        void balloon()
        {
            
            notifyIcon1.BalloonTipClicked -= notifyIcon1_BalloonTipClicked;
            notifyIcon1.BalloonTipClicked += notifyIcon1_BalloonTipClicked;
            notifyIcon1.ShowBalloonTip(1000000, "Timesheet", "Fill out your timesheet! Click here for a link", ToolTipIcon.Info);
        }
        void messageBox()
        {
            MessageBox.Show("Fill out your timesheet!");
            ProcessStartInfo sInfo = new ProcessStartInfo(timesheetUrl);
            Process.Start(sInfo);
        }
        void firePopup()
        {
            if (showBalloon)
                balloon();

            if (showMessageBox)
            {
                messageBox();
            }
        }

        void guessTime()
        {
            Form2 form = new Form2();
            form.Show();
        }


        void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(timesheetUrl);
            Process.Start(sInfo);
        }
       

        void checkPopup()
        {
            if (nextPopupTime < DateTime.Now)
            {
                TopMost = true;
                setNextPopup();

                firePopup();
                
            }
        }
        void loadSettings()
        {
            popupTime = Settings.Default.PopupTime;
            timesheetUrl = Settings.Default.TimesheetURL;
            showBalloon = Settings.Default.ShowBalloon;
            showMessageBox = Settings.Default.ShowMessageBox;
            startMinimized = Settings.Default.StartMinimized;
            logEvents = Settings.Default.LogEvents;
            logPath = Settings.Default.LogPath;
        }
        void saveSettings()
        {
            Settings.Default.PopupTime = popupTime;
            Settings.Default.TimesheetURL = timesheetUrl;
            Settings.Default.ShowBalloon = showBalloon;
            Settings.Default.ShowMessageBox = showMessageBox;
            Settings.Default.StartMinimized = startMinimized;
            Settings.Default.LogEvents = logEvents;
            Settings.Default.LogPath = logPath;

            Settings.Default.Save();
        }

        void setNextPopup()
        {
         
            nextPopupTime = DateTime.Today;
            nextPopupTime += popupTime;
            if (nextPopupTime < DateTime.Now)
                nextPopupTime = nextPopupTime.AddDays(1);
        }

        void bindControls()
        {
            saveValues = false;

            dtpTime.Value = (DateTime.Today + popupTime);
            txtUrl.Text = timesheetUrl;
            chkBalloon.Checked = showBalloon;
            chkMsg.Checked = showMessageBox;
            chkMin.Checked = startMinimized;
            chkLog.Checked = logEvents;
            txtLogPath.Text = logPath;

            saveValues = true;
        }

        void saveControls()
        {
            popupTime = dtpTime.Value.TimeOfDay;
            timesheetUrl = txtUrl.Text;
            showBalloon = chkBalloon.Checked;
            showMessageBox = chkMsg.Checked;
            startMinimized = chkMin.Checked;
            logEvents = chkLog.Checked;
            logPath = txtLogPath.Text;

            saveSettings();
        }

        public Form1()
        {
            
            InitializeComponent();
            loadSettings();
            setNextPopup();
            if (startMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                Hide();
            }
            notifyIcon1.ShowBalloonTip(3000, "Timesheet", "Now running, next reminder at: " + nextPopupTime.ToString(), ToolTipIcon.Info);

            logEvent("AppStart");
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        void logEvent(string eventText)
        {
            if (logEvents)
            {
                try
                {
                    System.IO.File.AppendAllText(logPath, String.Format("{0},{1}\r\n", DateTime.Now.ToString(), eventText));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem logging event: " + ex.Message);
                }
            }
        }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            logEvent(e.Reason.ToString());

            
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                Hide();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            realExit = true;
            Application.Exit();
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            guessTime();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!realExit)
            {
                this.WindowState = FormWindowState.Minimized;
                Hide();
                e.Cancel = true;
            }
            else
            {
                logEvent("AppClose");
            }
        }


        
        private void timer1_Tick(object sender, EventArgs e)
        {
            checkPopup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            firePopup();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            bindControls();
        }

        bool saveValues = false;

        private void dtpTime_ValueChanged(object sender, EventArgs e)
        {
            if(saveValues)
                saveControls();
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            if (saveValues)
                saveControls();
        }

        private void chkBalloon_CheckedChanged(object sender, EventArgs e)
        {
            if (saveValues)
                saveControls();
        }

        private void chkMsg_CheckedChanged(object sender, EventArgs e)
        {
            if (saveValues)
                saveControls();
        }
        private void chkMin_CheckedChanged(object sender, EventArgs e)
        {
            if (saveValues)
                saveControls();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveControls();
            Hide();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            try
            {

                ProcessStartInfo sInfo = new ProcessStartInfo(logPath);
                Process.Start(sInfo);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            guessTime();
        }

        private void txtLogPath_Leave(object sender, EventArgs e)
        {
            if (saveValues)
                saveControls();
        }

        
    }
}
