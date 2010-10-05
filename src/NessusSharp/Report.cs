using System;
namespace NessusSharp
{
    /// <summary>
    /// Represents a Nessus Report.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Status of the report.
        /// </summary>
        public enum Status
        {
            Running = 0,
            Paused,
            Completed
        }

        public Report ()
        {
        }
    }
}

