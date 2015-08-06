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

using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RaileyBuilder
{
    class ServerInstaller
    {
        private static readonly string[] ProgramFilesDirectories = new string[] {
            "Program Files (x86)",
            "Program Files"
        };

        private static readonly string GitSubdirectory = @"Git\bin\git.exe";
        private static readonly string MySQLSubdirectory = @"MySQL\MySQL Server 5.6\bin\mysql.exe";

        private static readonly string ServerURI = @"https://github.com/pmdcp/Server";

        public string ServerFolder { get; private set; }

        readonly string GitPath;
        readonly string MySQLPath;

        Action<string> logger;

        public ServerInstaller(string serverFolder, Action<string> logger)
        {
            this.ServerFolder = serverFolder;

            this.logger = logger;

            GitPath = FindAmbiguousExecutable(GitSubdirectory);
            MySQLPath = FindAmbiguousExecutable(MySQLSubdirectory);
        }

        private string FindAmbiguousExecutable(string subFolder)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                foreach (string programFilesDirectory in ProgramFilesDirectories)
                {
                    string testPath = Path.Combine(drive.Name, programFilesDirectory, subFolder);
                    if (File.Exists(testPath))
                    {
                        return testPath;
                    }
                }
            }

            throw new FileNotFoundException();
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

        private async Task<int> ExecuteAsync(string executable, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(executable, arguments);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = ServerFolder;
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

        public async Task UpdateServerAsync()
        {
            logger("Checking existing server installation...");

            if (IsInstallDirectoryEmpty())
            {
                logger("There is no server here! Install one first.");
                return;
            }

            logger("Pulling latest changes...");

            await ExecuteAsync(GitPath, "pull upstream --recurse-submodules");

            logger("Download complete!");

            bool buildResult = await PerformBuildAsync();
            if (!buildResult)
            {
                return;
            }

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

            await ExecuteAsync(GitPath, string.Format("clone --recursive {0} {1}", "\"" + ServerURI + "\"", "\"" + ServerFolder + "\""));

            logger("Download complete!");

            bool buildResult = await PerformBuildAsync();
            if (!buildResult)
            {
                return;
            }

            logger("Updating git repository...");

            await ExecuteAsync(GitPath, "remote remove origin");
            await ExecuteAsync(GitPath, string.Format("remote add upstream \"{0}\"", ServerURI));

            logger("Git repository updates complete");

            logger("Preparing initial configuration...");

            DatabaseConfigurationForm dbConfig = new DatabaseConfigurationForm();
            if (dbConfig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await WriteConfigurationFile(dbConfig.DatabaseUsername, dbConfig.DatabasePassword);
            }
            else
            {
                logger("Invalid database configuration entered.");
                return;
            }

            logger("Configuration file updated!");
            logger("Verifying database connection...");

            if (await TestDatabaseConnection(dbConfig.DatabaseUsername, dbConfig.DatabasePassword) == false)
            {
                return;
            }

            logger("Database connection test successful!");

            logger("Extracting database seed data...");

            if (Directory.Exists(Path.Combine(ServerFolder, "Temp")))
            {
                Directory.Delete(Path.Combine(ServerFolder, "Temp"), true);
            }
            using (FileStream file = new FileStream(Path.Combine(ServerFolder, "Content_Data.zip"), FileMode.Open))
            {
                using (ZipArchive zipArchive = new ZipArchive(file))
                {
                    zipArchive.ExtractToDirectory(Path.Combine(ServerFolder, "Temp"));
                }
            }

            logger("Extraction complete!");

            logger("Creating initial database schemas (this may take some time)");

            await ExecuteAsync(MySQLPath, string.Format("-u {0} -p{1} -e \"\\. \"{2}\"\"", dbConfig.DatabaseUsername, dbConfig.DatabasePassword, Path.Combine(ServerFolder, "Temp", "mdx_schemas.sql")));
            await ExecuteAsync(MySQLPath, string.Format("-u {0} -p{1} mdx_data -e \"\\. \"{2}\"\"", dbConfig.DatabaseUsername, dbConfig.DatabasePassword, Path.Combine(ServerFolder, "Temp", "mdx_data.sql")));
            await ExecuteAsync(MySQLPath, string.Format("-u {0} -p{1} mdx_players -e \"\\. \"{2}\"\"", dbConfig.DatabaseUsername, dbConfig.DatabasePassword, Path.Combine(ServerFolder, "Temp", "mdx_players.sql")));

            logger("Schemas created!");

            logger("Deleting temporary files...");

            Directory.Delete(Path.Combine(ServerFolder, "Temp"), true);

            logger("Temporary files deleted!");

            logger("Server installation complete!");
        }

        private async Task<bool> PerformBuildAsync()
        {
            logger("Starting build under (Release)...");

            string msBuildFolder = ReadRegistryKey(@"Software\Microsoft\MSBuild\ToolsVersions\4.0", "MSBuildToolsPath");
            if (string.IsNullOrEmpty(msBuildFolder))
            {
                logger("The correct version of MSBuild has not been found. Have you installed Visual Studio?");
                return false;
            }
            string msBuildPath = Path.Combine(msBuildFolder, "msbuild.exe");
            if (File.Exists(msBuildPath) == false)
            {
                logger("MSBuild has not been found. It appears to be installed, but some files are missing. Try reinstalling.");
                return false;
            }

            int result = await ExecuteAsync(msBuildPath, string.Format("\"{0}\" /p:Configuration=Release", Path.Combine(ServerFolder, "Server.sln")));

            if (result != 0)
            {
                logger("A build error has occured! Contact the PMDCP administrators for assistance.");
                return false;
            }

            logger("Build complete!");
            return true;
        }

        private async Task WriteConfigurationFile(string databaseUsername, string databasePassword)
        {
            string path = Path.Combine(ServerFolder, "Server", "bin", "Release", "Data", "config.xml");

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  "
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Data");

                xmlWriter.WriteStartElement("Settings");

                xmlWriter.WriteElementString("GamePort", "4001");
                xmlWriter.WriteElementString("DatabaseIP", "localhost");
                xmlWriter.WriteElementString("DatabasePort", "3306");
                xmlWriter.WriteElementString("DatabaseUser", databaseUsername);
                xmlWriter.WriteElementString("DatabasePassword", databasePassword);

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        private async Task<bool> TestDatabaseConnection(string username, string password)
        {
            string connectionString = string.Format(@"server=localhost;userid={0};password={1};", username, password);
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                logger(string.Format("MySQL connection opened. Server version: {0}", connection.ServerVersion));
                return true;
            }
            catch (MySqlException)
            {
                logger("Unable to open connection to MySQL. Check the username and password, and try again");
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
