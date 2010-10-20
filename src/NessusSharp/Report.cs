using System;
namespace NessusSharp
{
    /// <summary>
    /// Represents a Nessus Report.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Status of the scan the report is for.
        /// </summary>
        public enum ScanStatus
        {
            Running = 0,
            Paused,
            Completed
        }

        public DateTime TimeStamp { get; private set; }
        public ScanStatus Status { get; private set; }
        public string Name { get; private set; }

        public Report(DateTime timeStamp, ScanStatus status, string name)
        {
            TimeStamp = timeStamp;
            Status = status;
            Name = name;
        }
    }
}

