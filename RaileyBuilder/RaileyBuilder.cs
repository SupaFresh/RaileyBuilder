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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RaileyBuilder
{
    public partial class RaileyBuilder : Form
    {
        private int lastDependencyY;

        public RaileyBuilder()
        {
            InitializeComponent();

            lastDependencyY = progressBar.Location.Y + progressBar.Height + 5;
        }

        private delegate void UpdateProgressDelegate(string message, int value);

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

        private delegate void ReportDependencyDelegare(string name, string downloadUrl);

        private void ReportDependency(string name, string downloadUrl)
        {
            if (InvokeRequired)
            {
                Invoke(new ReportDependencyDelegare(ReportDependency), name, downloadUrl);
            }
            else
            {
                Label label = new Label
                {
                    AutoSize = true,
                    Text = "Missing: " + name,
                    Location = new Point(10, lastDependencyY)
                };

                LinkLabel linkLabel = new LinkLabel
                {
                    AutoSize = true,
                    Text = downloadUrl,
                    Location = new Point(label.Location.X + label.Width + 10, lastDependencyY)
                };

                Controls.Add(label);
                Controls.Add(linkLabel);

                int maxHeight = System.Math.Max(label.Height, linkLabel.Height);

                Height += maxHeight;

                lastDependencyY += System.Math.Max(label.Height, linkLabel.Height) + 10;
            }
        }

        private void DisableInstallerOptions()
        {
            installServerButton.Enabled = false;
            updateServerButton.Enabled = false;
            installClientButton.Enabled = false;
            updateClientButton.Enabled = false;
        }

        private void EnableInstallerOptions()
        {
            installServerButton.Enabled = true;
            updateServerButton.Enabled = true;
            installClientButton.Enabled = true;
            updateClientButton.Enabled = true;
        }

        private void browseServerFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                targetFolderTextBox.Text = fbd.SelectedPath;
            }
        }

        private async void installServerButton_Click(object sender, EventArgs e)
        {
            DisableInstallerOptions();
            string logPath = "Log.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            Reporter reporter = new Reporter(UpdateProgress, ReportDependency, logPath);
            ServerInstaller serverInstaller = new ServerInstaller(targetFolderTextBox.Text, reporter);

            await serverInstaller.InstallServerAsync();

            EnableInstallerOptions();
        }

        private async void updateServerButton_Click(object sender, EventArgs e)
        {
            DisableInstallerOptions();
            string logPath = "Log.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            Reporter reporter = new Reporter(UpdateProgress, ReportDependency, logPath);
            ServerInstaller serverInstaller = new ServerInstaller(targetFolderTextBox.Text, reporter);

            await serverInstaller.UpdateServerAsync();

            EnableInstallerOptions();
        }

        private async void installClientButton_Click(object sender, EventArgs e)
        {
            DisableInstallerOptions();
            string logPath = "Log.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            Reporter reporter = new Reporter(UpdateProgress, ReportDependency, logPath);
            ClientInstaller clientInstaller = new ClientInstaller(targetFolderTextBox.Text, reporter);

            await clientInstaller.InstallClientAsync();

            EnableInstallerOptions();
        }

        private async void updateClientButton_Click(object sender, EventArgs e)
        {
            DisableInstallerOptions();
            string logPath = "Log.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            Reporter reporter = new Reporter(UpdateProgress, ReportDependency, logPath);
            ClientInstaller clientInstaller = new ClientInstaller(targetFolderTextBox.Text, reporter);

            await clientInstaller.UpdateClientAsync();

            EnableInstallerOptions();
        }
    }
}