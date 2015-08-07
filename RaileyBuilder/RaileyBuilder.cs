// This file is part of Mystery Dungeon eXtended.

// Copyright (C) 2015 Pikablu

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

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

        delegate void UpdateProgressDelegate(string message, int value);
        private void UpdateProgress(string message, int value)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateProgressDelegate(UpdateProgress), message, value);
            }
            else
            {
                progressBar.Value = value;
                progressLabel.Text = message;
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
            ServerInstaller serverInstaller = new ServerInstaller(serverFolderPathTextBox.Text, LogMessage, UpdateProgress);

            await serverInstaller.InstallServerAsync();

            EnableServerOptions();
        }

        private async void updateServerButton_Click(object sender, EventArgs e)
        {
            DisableServerOptions();
            ServerInstaller serverInstaller = new ServerInstaller(serverFolderPathTextBox.Text, LogMessage, UpdateProgress);

            await serverInstaller.UpdateServerAsync();

            EnableServerOptions();
        }
    }
}
