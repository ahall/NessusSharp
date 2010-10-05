using System;
using System.Collections.Generic;

namespace DotNessus
{
    /// <summary>
    /// Represents a Nessus scan.
    /// </summary>
    public class Scan
    {
        /// <summary>
        /// The scan targets - can be hostnames, IPs, CIDR masks and ranges.
        /// </summary>
        public List<string> Targets { get; private set; }

        /// <summary>
        /// Unique identifier for this scan, this is corresponds to a report name
        /// as report is a completed scan.
        /// </summary>
        public string Uuid { get; internal set; }

        /// <summary>
        /// The name of the scan.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Start time of the scan - in UTC.
        /// </summary>
        public DateTime StartTime { get; internal set; }

        /// <summary>
        /// Returns the targets in the format the Nessus XMLrpc protocol expects it to be.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string EncodeTargets()
        {
            return string.Join(",", Targets.ToArray());
        }

        public Scan(string name)
        {
            Name = name;
            Targets = new List<string>();
        }
    }
}

