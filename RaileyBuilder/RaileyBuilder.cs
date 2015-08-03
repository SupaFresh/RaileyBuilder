using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RaileyBuilder
{
    public partial class RaileyBuilder : Form
    {
        public RaileyBuilder()
        {
            InitializeComponent();
        }

        delegate void LogMessageDelegate(string message);
        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new LogMessageDelegate(LogMessage), message);
            }
            else
            {
                logBox.Text = logBox.Text + Environment.NewLine + "[" + DateTime.Now.ToLongTimeString() + "] " + message;
            }
        }

        private void DisableServerOptions()
        {
            installServerButton.Enabled = false;
            updateServerButton.Enabled = false;
        }

        private void EnableServerOptions()
        {
            installServerButton.Enabled = true;
            updateServerButton.Enabled = true;
        }

        private void browseServerFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                serverFolderPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private async void installServerButton_Click(object sender, EventArgs e)
        {
            DisableServerOptions();
            ServerInstaller serverInstaller = new ServerInstaller(serverFolderPathTextBox.Text, LogMessage);

            await serverInstaller.InstallServerAsync();

            EnableServerOptions();
        }

        private async void updateServerButton_Click(object sender, EventArgs e)
        {
            DisableServerOptions();
            ServerInstaller serverInstaller = new ServerInstaller(serverFolderPathTextBox.Text, LogMessage);

            await serverInstaller.UpdateServerAsync();

            EnableServerOptions();
        }
    }
}
