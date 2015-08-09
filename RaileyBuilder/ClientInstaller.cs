using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaileyBuilder
{
    class ClientInstaller : BaseInstaller
    {
        private static readonly string ClientURI = @"https://github.com/pmdcp/Client";

        public ClientInstaller(string targetDirectory, Reporter reporter)
            : base(targetDirectory, reporter)
        {
        }

        public async Task UpdateClientAsync()
        {
            reporter.WriteToLog("Checking existing client installation...");
            reporter.UpdateProgress("Checking existing client installation...", 0);

            if (IsInstallDirectoryEmpty())
            {
                reporter.ReportError("There is no client here! Install one first.");
                return;
            }

            reporter.WriteToLog("Pulling latest changes...");
            reporter.UpdateProgress("Pulling latest changes...", 15);

            int result = await ExecuteAsync(GitPath, "pull --recurse-submodules --ff-only upstream master");
            if (result == 1)
            {
                reporter.ReportError("Unable to pull latest changes and merge. Have you made edits to your client?");
                return;
            }

            result = await ExecuteAsync(GitPath, "checkout master");
            if (result == 1)
            {
                reporter.ReportError("Unable to checkout latest version. Have you made edits to your client?");
                return;
            }

            reporter.WriteToLog("Download complete!");

            reporter.UpdateProgress("Building client...", 70);
            bool buildResult = await PerformBuildAsync("Client.sln", "Release");
            if (!buildResult)
            {
                return;
            }

            reporter.UpdateProgress("Client update has been completed!", 100);
        }

        public async Task InstallClientAsync()
        {
            reporter.UpdateProgress("Starting client installation", 0);
            reporter.WriteToLog("Starting client installation");
            reporter.WriteToLog("Git Path: " + GitPath);
            reporter.WriteToLog("Checking input data...");

            if (!(await CheckDependencies()))
            {
                return;
            }

            if (!IsInstallDirectoryEmpty())
            {
                reporter.ReportError("Installation directory is not empty. Please select an empty directory and try again!");
                return;
            }

            if (Directory.Exists(TargetDirectory) == false)
            {
                Directory.CreateDirectory(TargetDirectory);
            }

            reporter.WriteToLog("Preparing to download latest client files...");
            reporter.UpdateProgress("Downloading latest client files...", 10);

            await ExecuteAsync(GitPath, string.Format("clone --recursive {0} {1}", "\"" + ClientURI + "\"", "\"" + TargetDirectory + "\""));

            reporter.WriteToLog("Download complete!");

            reporter.UpdateProgress("Building client (Release)", 30);
            bool buildResult = await PerformBuildAsync("Client.sln", "Release");
            if (!buildResult)
            {
                return;
            }

            reporter.UpdateProgress("Configuring local settings...", 60);
            reporter.WriteToLog("Updating git repository...");

            await ExecuteAsync(GitPath, "remote remove origin");
            await ExecuteAsync(GitPath, string.Format("remote add upstream \"{0}\"", ClientURI));

            reporter.WriteToLog("Git repository updates complete");

            reporter.WriteToLog("Client installation complete!");
            reporter.UpdateProgress("Client installation complete!", 100);

            Process.Start(Path.Combine(TargetDirectory, "Client", "bin", "Release"));
        }
    }
}
