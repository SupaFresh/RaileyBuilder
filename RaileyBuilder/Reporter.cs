using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaileyBuilder
{
    class Reporter
    {
        Action<string, int> updateProgressDelegate;

        public Reporter(Action<string, int> updateProgressDelegate)
        {
            this.updateProgressDelegate = updateProgressDelegate;
        }

        public void UpdateProgress(string message, int value)
        {
            updateProgressDelegate(message, value);
        }

        public void WriteToLog(string message)
        {

        }

        public void WriteProgramOutputToLog(string executable, string arguments, string output)
        {

        }

        public void WriteExceptionToLog(Exception ex)
        {

        }
    }
}
