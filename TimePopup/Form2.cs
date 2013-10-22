using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimePopup.Properties;

namespace TimePopup
{
    
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void btnGuess_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            string path = Settings.Default.LogPath;

            DateTime day = dtp.Value.Date;

            List<LogEntry> entries = new List<LogEntry>();


            foreach (string line in System.IO.File.ReadAllLines(path))
            {
                string[] spl = line.Split(',');
                if (spl.Length > 1)
                {
                    LogEntry ent = new LogEntry();
                    if (DateTime.TryParse(spl[0], out ent.Date))
                    {
                        ent.Event = spl[1];
                        entries.Add(ent);
                    }
                }
            }
            List<LogEntry> today = entries.Where(l => l.Date.Date == day).OrderBy(l => l.Date).ToList();

            foreach (LogEntry ent in today)
            {
                lstLog.Items.Add(new ListViewItem(new string[] { ent.Date.ToString(), ent.Event }));
            }

            LogEntry lowest = today.FirstOrDefault(l => l.Event == "SessionUnlock" || l.Event == "AppStart");
            LogEntry highest = today.LastOrDefault(l => l.Event == "SessionLock" || l.Event == "AppClose");
            if (lowest == null || highest == null)
                return;

            TimeSpan diff = highest.Date - lowest.Date;

            
            txtFirst.Text = lowest.Date.TimeOfDay.ToString();
            if (lowest.Event == "AppStart")
            {
                lblStart.Visible = true;
            }
            else
            {
                lblStart.Visible = false;
            }
            txtLast.Text = highest.Date.TimeOfDay.ToString();
            if (highest.Event == "AppClose")
            {
                lblClose.Visible = true;
            }
            else
            {
                lblClose.Visible = false;
            }
            txtHours.Text = Math.Round(diff.TotalHours,2).ToString();
            
        }
    }
    class LogEntry
    {
        public DateTime Date;
        public string Event;
    }
}
