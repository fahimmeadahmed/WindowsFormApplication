using System;
using System.IO;
using System.ServiceProcess;
using System.Security.AccessControl;
using System.Security.Principal;

namespace WindowsServiceSample
{
    public partial class Service1 : ServiceBase
    {
        private DateTime _startTime;
        private string _logFilePath = @"D:\ServiceLogs\Service1Log.txt";

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _startTime = DateTime.Now;
            string message = $"{_startTime}: Service has started.";
            LogMessage(message);
        }

        protected override void OnStop()
        {
            DateTime stopTime = DateTime.Now;
            TimeSpan runningTime = stopTime - _startTime;
            string message = $"{stopTime}: Service has stopped. Service was running for {runningTime.TotalSeconds} seconds.";
            LogMessage(message);
        }

        private void LogMessage(string message)
        {
            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));

                // Grant read and write permissions if needed
                SetFilePermissions(_logFilePath);

                // Write the log message
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, perhaps by logging to Event Viewer
            }
        }

        private void SetFilePermissions(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();

                // Grant read/write permissions to all users
                fileSecurity.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.Read | FileSystemRights.Write,
                    AccessControlType.Allow));

                fileInfo.SetAccessControl(fileSecurity);
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, perhaps by logging to Event Viewer
            }
        }
    }
}
