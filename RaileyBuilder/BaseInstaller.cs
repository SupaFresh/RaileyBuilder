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
    class BaseInstaller
    {
        protected static readonly string[] ProgramFilesDirectories = new string[] {
            "Program Files (x86)",
            "Program Files"
        };

        protected static readonly string GitSubdirectory = @"Git\bin\git.exe";
        protected static readonly string MySQLSubdirectory = @"MySQL\MySQL Server 5.6\bin\mysql.exe";

        public string TargetDirectory { get; private set; }

        public readonly string GitPath;
        public readonly string MySQLPath;

        protected Reporter reporter { get; private set; }

        public BaseInstaller(string targetDirectory, Reporter reporter)
        {
            this.TargetDirectory = targetDirectory;

            this.reporter = reporter;

            GitPath = FindAmbiguousExecutable(GitSubdirectory);
            MySQLPath = FindAmbiguousExecutable(MySQLSubdirectory);
        }

        public bool IsInstallDirectoryEmpty()
        {
            if (Directory.Exists(TargetDirectory) == false)
            {
                return true;
            }

            if (Directory.EnumerateFiles(TargetDirectory).FirstOrDefault() == null && Directory.EnumerateDirectories(TargetDirectory).FirstOrDefault() == null)
            {
                return true;
            }

            return false;
        }

        protected string FindAmbiguousExecutable(string subFolder)
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

            throw new FileNotFoundException("Unable to find: " + subFolder);
        }

        protected async Task<int> ExecuteAsync(string executable, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(executable, arguments);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = TargetDirectory;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process exec = Process.Start(startInfo);
            string output = await exec.StandardOutput.ReadToEndAsync();
            string error = await exec.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(error))
            {
                output += Environment.NewLine + Environment.NewLine + "Error:" + Environment.NewLine + await exec.StandardError.ReadToEndAsync();
            }

            reporter.WriteProgramOutputToLog(executable, arguments, output);

            return exec.ExitCode;
        }

        protected string ReadRegistryKey(string subKeyName, string keyName)
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

        protected async Task<bool> CheckDependencies()
        {
            if (!File.Exists(GitPath))
            {
                reporter.ReportMissingDependency("Git", "https://msysgit.github.io/");
                reporter.ReportError("Unable to find Git client. Make sure you've installed msysgit!");
                return false;
            }

            if (!File.Exists(MySQLPath))
            {
                reporter.ReportMissingDependency("MySQL", "http://dev.mysql.com/downloads/windows/installer/5.6.html");
                reporter.ReportError("Unable to find MySQL. Make sure you've installed MySQL!");
                return false;
            }

            return true;
        }

        protected async Task<bool> PerformBuildAsync(string relativePath, string configuration)
        {
            reporter.WriteToLog(string.Format("Starting build under ({0})...", configuration));

            string msBuildFolder = ReadRegistryKey(@"Software\Microsoft\MSBuild\ToolsVersions\4.0", "MSBuildToolsPath");
            if (string.IsNullOrEmpty(msBuildFolder))
            {
                reporter.ReportError("The correct version of MSBuild has not been found. Have you installed Visual Studio?");
                return false;
            }
            string msBuildPath = Path.Combine(msBuildFolder, "msbuild.exe");
            if (File.Exists(msBuildPath) == false)
            {
                reporter.ReportError("MSBuild has not been found. It appears to be installed, but some files are missing. Try reinstalling.");
                return false;
            }

            int result = await ExecuteAsync(msBuildPath, string.Format("\"{0}\" /p:Configuration={1}", Path.Combine(TargetDirectory, relativePath), configuration));

            if (result != 0)
            {
                reporter.ReportError("A build error has occured! Contact the PMDCP administrators for assistance.");
                return false;
            }

            reporter.WriteToLog("Build complete!");
            return true;
        }
    }
}
