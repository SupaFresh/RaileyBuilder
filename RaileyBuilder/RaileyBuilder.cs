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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RaileyBuilder
{
    public partial class RaileyBuilder : Form
    {
        int lastDependencyY;

        public RaileyBuilder()
        {
            InitializeComponent();

            lastDependencyY = progressBar.Location.Y + progressBar.Height + 5;
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
                if (value != -1)
                {
                    progressBar.Value = value;
                }
                progressLabel.Text = message;
            }
        }

        delegate void ReportDependencyDelegare(string name, string downloadUrl);
        private void ReportDependency(string name, string downloadUrl)
        {
            if (InvokeRequired)
            {
                Invoke(new ReportDependencyDelegare(ReportDependency), name, downloadUrl);
            }
            else
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Text = "Missing: " + name;
                label.Location = new Point(10, lastDependencyY);

                LinkLabel linkLabel = new LinkLabel();
                linkLabel.AutoSize = true;
                linkLabel.Text = downloadUrl;
                linkLabel.Location = new Point(label.Location.X + label.Width + 10, lastDependencyY);

                this.Controls.Add(label);
                this.Controls.Add(linkLabel);

                int maxHeight = System.Math.Max(label.Height, linkLabel.Height);

                this.Height += maxHeight;

                lastDependencyY += System.Math.Max(label.Height, linkLabel.Height) + 10;
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
            string logPath = "Log.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            Reporter reporter = new Reporter(UpdateProgress, ReportDependency, logPath);
            ServerInstaller serverInstaller = new ServerInstaller(serverFolderPathTextBox.Text, reporter);

            await serverInstaller.InstallServerAsync();

            EnableServerOptions();
        }

        private async void updateServerButton_Click(object sender, EventArgs e)
        {
            DisableServerOptions();
            string logPath = "Log.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            Reporter reporter = new Reporter(UpdateProgress, ReportDependency, logPath);
            ServerInstaller serverInstaller = new ServerInstaller(serverFolderPathTextBox.Text, reporter);

            await serverInstaller.UpdateServerAsync();

            EnableServerOptions();
        }
    }
}
