using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace AlarmFromLog
{
    public partial class AlarmForm : Form
    {

        delegate void SetTextCallback();

        public Thread readLogThread;

        public Thread alarmWorkThread;

        public AlarmForm()
        {
            InitializeComponent();
        }

        private void AlarmForm_Load(object sender, EventArgs e)
        {
            IniFile ini = new IniFile(Directory.GetCurrentDirectory() + "\\AlarmFromLog.ini");
            String logFilePath = ini.IniReadValue("initParams", "logFilePath");
            String logReadIntervalMilliseconds = ini.IniReadValue("initParams", "logReadIntervalMilliseconds");
            String alarmKeys = ini.IniReadValue("initParams", "alarmKeys");
            String alarmText = ini.IniReadValue("initParams", "alarmText");
            String okText = ini.IniReadValue("initParams", "okText");

            if (logFilePath == null || logFilePath == String.Empty)
            {
                MessageBox.Show("Parameter 'logFilePath' not exists in AlarmFromLog.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (logReadIntervalMilliseconds == null || logReadIntervalMilliseconds == String.Empty)
            {
                MessageBox.Show("Parameter 'logReadIntervalMilliseconds' not exists in AlarmFromLog.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (alarmKeys == null || alarmKeys == String.Empty)
            {
                MessageBox.Show("Parameter 'alarmKeys' not exists in AlarmFromLog.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (alarmText == null || alarmText == String.Empty)
            {
                MessageBox.Show("Parameter 'alarmText' not exists in AlarmFromLog.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (okText == null || okText == String.Empty)
            {
                MessageBox.Show("Parameter 'okText' not exists in AlarmFromLog.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Common.logFilePath = logFilePath;
            Common.logReadIntervalMilliseconds = Convert.ToInt32(logReadIntervalMilliseconds);
            Common.alarmKeys = alarmKeys.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Common.alarmText = alarmText;
            Common.okText = okText;

            Panel panel = (Panel)this.Controls["panel"];

            Common.panelOriginColor = panel.BackColor;


            Label labelStatus = (Label)panel.Controls["labelStatus"];

            labelStatus.Text = Common.okText;

            readLogThread = new Thread(LogReader.doRead);
            readLogThread.Start(this);
        }

        private void AlarmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.readLogThread != null)
            this.readLogThread.Abort();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        public void doAlarm()
        {

            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(doAlarm);
                this.Invoke(d, new object[] {});
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;

                this.Activate();

                Panel panel = (Panel)this.Controls["panel"];

                Label labelStatus = (Label)panel.Controls["labelStatus"];

                labelStatus.Text = Common.alarmText + " " + Common.logFileAlarmLines;

                Button buttonReset = (Button)this.Controls["buttonReset"];

                buttonReset.Visible = true;
            }

        }

        public void reset()
        {
            if (this.readLogThread != null)
                this.readLogThread.Abort();

            Panel panel = (Panel)this.Controls["panel"];

            panel.BackColor = Common.panelOriginColor;

            Label labelStatus = (Label)panel.Controls["labelStatus"];

            labelStatus.Text = Common.okText;

            Button buttonReset = (Button)this.Controls["buttonReset"];

            buttonReset.Visible = false;

            Common.logFileAlarmLines = null;

            readLogThread = new Thread(LogReader.doRead);
            readLogThread.Start(this);

        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            reset();
        }

    }
}
