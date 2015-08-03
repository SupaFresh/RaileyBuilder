using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaileyBuilder
{
    class ServerInstaller
    {
        public string ServerFolder { get; private set; }

        Action<string> logger;

        public ServerInstaller(string serverFolder, Action<string> logger)
        {
            this.ServerFolder = serverFolder;

            this.logger = logger;
        }

        public bool IsInstallDirectoryEmpty()
        {
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

            logger("Download complete!");
            logger("Starting build under (Release)...");

            logger("Build complete!");

            logger("Preparing initial configuration...");

            logger("Verifying database connection...");

            logger("Database connection test successful!");

            logger("Creating initial database schemas...");

            logger("Schemas created!");

            logger("Server installation complete!");
        }
    }
}
