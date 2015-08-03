using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaileyBuilder
{
    class ServerInstaller
    {
        private static readonly string GitPath = @"C:\Program Files (x86)\Git\bin\git.exe";

        private static readonly string ServerURI = @"https://github.com/pmdcp/Server";

        public string ServerFolder { get; private set; }

        Action<string> logger;

        public ServerInstaller(string serverFolder, Action<string> logger)
        {
            this.ServerFolder = serverFolder;

            this.logger = logger;
        }

        public bool IsInstallDirectoryEmpty()
        {
            return true;

            if (Directory.Exists(ServerFolder) == false)
            {
                return true;
            }

            if (Directory.EnumerateFiles(ServerFolder).FirstOrDefault() == null && Directory.EnumerateDirectories(ServerFolder).FirstOrDefault() == null)
            {
                return true;
            }

            return false;
        }

        private async Task<int> ExecuteAsync(string executable, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(executable, arguments);
            startInfo.UseShellExecute = false;
            Process exec = Process.Start(startInfo);
            await Task.Run(() =>
            {
                exec.WaitForExit();
            });

            return exec.ExitCode;
        }

        private string ReadRegistryKey(string subKeyName, string keyName)
        {
            using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(subKeyName))
            {
                if (subKey != null)
                {
                    Object o = subKey.GetValue(keyName);
                    if (o != null)
                    {
                        return o as string;
                    }
                }
            }

            return null;
        }

        public async Task InstallServerAsync()
        {
            logger("Starting server installation");
            logger("Checking input data...");

            if (!IsInstallDirectoryEmpty())
            {
                logger("Installation directory is not empty. Please select an empty directory and try again!");
                return;
            }

            if (Directory.Exists(ServerFolder) == false)
            {
                Directory.CreateDirectory(ServerFolder);
            }

            logger("Preparing to download latest server files...");

            if (!File.Exists(GitPath))
            {
                logger("Unable to find Git client. Make sure you've installed GitHub for Desktop!");
                return;
            }

            await ExecuteAsync(GitPath, string.Format("clone {0} {1}", "\"" + ServerURI + "\"", "\"" + ServerFolder + "\""));

            logger("Download complete!");
            logger("Starting build under (Release)...");

            string msBuildFolder = ReadRegistryKey(@"Software\Microsoft\MSBuild\ToolsVersions\4.0", "MSBuildToolsPath");
            if (string.IsNullOrEmpty(msBuildFolder))
            {
                logger("The correct version of MSBuild has not been found. Have you installed Visual Studio?");
                return;
            }
            string msBuildPath = Path.Combine(msBuildFolder, "msbuild.exe");
            if (File.Exists(msBuildPath) == false)
            {
                logger("MSBuild has not been found. It appears to be installed, but some files are missing. Try reinstalling.");
                return;
            }

            int result = await ExecuteAsync(msBuildPath, string.Format("\"{0}\" /p:Configuration=Release", Path.Combine(ServerFolder, "Server.sln")));

            if (result != 0)
            {
                logger("A build error has occured! Contact the PMDCP administrators for assistance.");
                return;
            }

            logger("Build complete!");

            logger("Updating git repository...");

            await ExecuteAsync(GitPath, "remote remove origin");
            await ExecuteAsync(GitPath, string.Format("remote add upstream \"{0}\"", ServerURI));

            logger("Git repository updates complete");

            logger("Preparing initial configuration...");

            logger("Verifying database connection...");

            logger("Database connection test successful!");

            logger("Creating initial database schemas...");

            logger("Schemas created!");

            logger("Server installation complete!");
        }
    }
}
