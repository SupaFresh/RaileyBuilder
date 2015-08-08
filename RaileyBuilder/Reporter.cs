using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaileyBuilder
{
    class Reporter
    {
        Action<string, int> updateProgressDelegate;
        string logFilePath;

        public Reporter(Action<string, int> updateProgressDelegate, string logFilePath)
        {
            this.updateProgressDelegate = updateProgressDelegate;
            this.logFilePath = logFilePath;
        }

        public void UpdateProgress(string message, int value)
        {
            updateProgressDelegate(message, value);
        }

        public void WriteToLog(string message)
        {
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("[" + DateTime.Now.ToString() + "] " + message);
                }
            }
        }

        public void WriteProgramOutputToLog(string executable, string arguments, string output)
        {
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("[" + DateTime.Now.ToString() + "] *** Executing: \"" + executable + "\" " + arguments);
                    streamWriter.WriteLine("Output:");
                    streamWriter.WriteLine(output);
                }
            }
        }

        public void WriteExceptionToLog(Exception ex)
        {
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("[" + DateTime.Now.ToString() + "] *** Exception:");
                    streamWriter.WriteLine(ex.ToString());
                }
            }
        }

        public void ReportError(string errorMessage)
        {
            WriteToLog("ERROR: " + errorMessage);
            UpdateProgress("ERROR: " + errorMessage, -1);
        }
    }
}
