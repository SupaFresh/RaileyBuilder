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
    class ServerInstaller : BaseInstaller
    {
        private static readonly string ServerURI = @"https://github.com/pmdcp/Server";

        public ServerInstaller(string targetDirectory, Reporter reporter)
            : base(targetDirectory, reporter)
        {
        }

        public async Task UpdateServerAsync()
        {
            reporter.WriteToLog("Checking existing server installation...");
            reporter.UpdateProgress("Checking existing server installation...", 0);

            if (IsInstallDirectoryEmpty())
            {
                reporter.ReportError("There is no server here! Install one first.");
                return;
            }

            reporter.WriteToLog("Pulling latest changes...");
            reporter.UpdateProgress("Pulling latest changes...", 15);

            int result = await ExecuteAsync(GitPath, "pull --recurse-submodules --ff-only upstream master");
            if (result == 1)
            {
                reporter.ReportError("Unable to pull latest changes and merge. Have you made edits to your server?");
                return;
            }

            result = await ExecuteAsync(GitPath, "checkout master");
            if (result == 1)
            {
                reporter.ReportError("Unable to checkout latest version. Have you made edits to your server?");
                return;
            }

            reporter.WriteToLog("Download complete!");

            reporter.UpdateProgress("Building server...", 70);
            bool buildResult = await PerformBuildAsync("Server.sln", "Release");
            if (!buildResult)
            {
                return;
            }

            reporter.UpdateProgress("Server update has been completed!", 100);
        }

        public async Task InstallServerAsync()
        {
            reporter.UpdateProgress("Starting server installation", 0);
            reporter.WriteToLog("Starting server installation");
            reporter.WriteToLog("MySQL Path: " + MySQLPath);
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

            reporter.WriteToLog("Preparing to download latest server files...");
            reporter.UpdateProgress("Downloading latest server files...", 10);

            await ExecuteAsync(GitPath, string.Format("clone --recursive {0} {1}", "\"" + ServerURI + "\"", "\"" + TargetDirectory + "\""));

            reporter.WriteToLog("Download complete!");

            reporter.UpdateProgress("Building server (Release)", 30);
            bool buildResult = await PerformBuildAsync("Server.sln", "Release");
            if (!buildResult)
            {
                return;
            }

            reporter.UpdateProgress("Configuring local settings...", 60);
            reporter.WriteToLog("Updating git repository...");

            await ExecuteAsync(GitPath, "remote remove origin");
            await ExecuteAsync(GitPath, string.Format("remote add upstream \"{0}\"", ServerURI));

            reporter.WriteToLog("Git repository updates complete");

            reporter.WriteToLog("Preparing initial configuration...");

            DatabaseConfigurationForm dbConfig = new DatabaseConfigurationForm();
            if (dbConfig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await WriteConfigurationFile(dbConfig.DatabaseUsername, dbConfig.DatabasePassword, dbConfig.DatabasePort);
            }
            else
            {
                reporter.ReportError("Invalid database configuration entered.");
                return;
            }

            reporter.WriteToLog("Configuration file updated!");

            reporter.UpdateProgress("Testing database connection...", 70);
            reporter.WriteToLog("Verifying database connection...");

            if (await TestDatabaseConnection(dbConfig.DatabaseUsername, dbConfig.DatabasePassword, dbConfig.DatabasePort) == false)
            {
                return;
            }

            reporter.WriteToLog("Database connection test successful!");

            reporter.UpdateProgress("Extracting database seed data...", 80);
            reporter.WriteToLog("Extracting database seed data...");

            if (Directory.Exists(Path.Combine(TargetDirectory, "Temp")))
            {
                Directory.Delete(Path.Combine(TargetDirectory, "Temp"), true);
            }
            using (FileStream file = new FileStream(Path.Combine(TargetDirectory, "Content_Data.zip"), FileMode.Open))
            {
                using (ZipArchive zipArchive = new ZipArchive(file))
                {
                    zipArchive.ExtractToDirectory(Path.Combine(TargetDirectory, "Temp"));
                }
            }

            reporter.WriteToLog("Extraction complete!");

            reporter.WriteToLog("Creating initial database schemas (this may take some time)");

            reporter.UpdateProgress("Importing database schemas", 85);
            await ExecuteAsync(MySQLPath, string.Format("-u {0} -p{1} -e \"\\. {2}\"", dbConfig.DatabaseUsername, dbConfig.DatabasePassword, Path.Combine(TargetDirectory, "Temp", "mdx_schemas.sql")));
            reporter.UpdateProgress("Importing game data", 90);
            await ExecuteAsync(MySQLPath, string.Format("-u {0} -p{1} -e \"\\. {2}\"", dbConfig.DatabaseUsername, dbConfig.DatabasePassword, Path.Combine(TargetDirectory, "Temp", "mdx_data.sql")));
            reporter.UpdateProgress("Importing player data", 95);
            await ExecuteAsync(MySQLPath, string.Format("-u {0} -p{1} -e \"\\. {2}\"", dbConfig.DatabaseUsername, dbConfig.DatabasePassword, Path.Combine(TargetDirectory, "Temp", "mdx_players.sql")));

            reporter.WriteToLog("Schemas created!");

            reporter.UpdateProgress("Cleaning up...", 99);
            reporter.WriteToLog("Deleting temporary files...");

            Directory.Delete(Path.Combine(TargetDirectory, "Temp"), true);

            reporter.WriteToLog("Temporary files deleted!");

            reporter.WriteToLog("Server installation complete!");
            reporter.UpdateProgress("Server installation complete!", 100);
        }

        private async Task WriteConfigurationFile(string databaseUsername, string databasePassword, int databasePort)
        {
            string path = Path.Combine(TargetDirectory, "Server", "bin", "Release", "Data", "config.xml");

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
                xmlWriter.WriteElementString("DatabasePort", databasePort.ToString());
                xmlWriter.WriteElementString("DatabaseUser", databaseUsername);
                xmlWriter.WriteElementString("DatabasePassword", databasePassword);

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        private async Task<bool> TestDatabaseConnection(string username, string password, int port)
        {
            string connectionString = string.Format(@"server=localhost;port={0};userid={1};password={2};", port, username, password);
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                reporter.WriteToLog(string.Format("MySQL connection opened. Server version: {0}", connection.ServerVersion));
                return true;
            }
            catch (MySqlException ex)
            {
                reporter.ReportError("Unable to open connection to MySQL. Check the username and password, and try again.");
                reporter.WriteExceptionToLog(ex);
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
