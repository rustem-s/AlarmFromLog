using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace AlarmFromLog
{
    class LogReader
    {
        public static void doRead(object alarmForm)
        {

            while (true)
            {
                FileInfo logFile = new FileInfo(Common.logFilePath);
                long lastLogLength = logFile.Length;

                Thread.Sleep(Common.logReadIntervalMilliseconds); 
                string newFileLines;
                var fs = new FileStream(Common.logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (var sr = new StreamReader(fs))
                {
                    sr.BaseStream.Position = lastLogLength;
                    newFileLines = sr.ReadToEnd();

                    //doAlarm((AlarmForm)alarmForm);
                    //break;

                    Boolean keysFound = true;

                    foreach (string s in Common.alarmKeys)
	                {
                        
	                    if(newFileLines.IndexOf(s) == -1) 
                        {
                            keysFound = false;
                            break;
                        }
	                }

                    if (keysFound)
                    {
                        Common.logFileAlarmLines = newFileLines;
                        doAlarm((AlarmForm)alarmForm);
                        break;
                    }

                    if (newFileLines == null || newFileLines == String.Empty)
                        continue;

                }
            }
        }

        public static void doAlarm(AlarmForm alarmForm)
        {
            alarmForm.doAlarm();

            Panel panel = (Panel)alarmForm.Controls["panel"];

            Color originColor = panel.BackColor;

            while (true)
            {
                Thread.Sleep(500);
                panel.BackColor = System.Drawing.Color.Red;
                Thread.Sleep(500);
                panel.BackColor = Common.panelOriginColor;
            }

        }

    }
}