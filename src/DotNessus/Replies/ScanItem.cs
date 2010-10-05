using System;
using System.Runtime.Serialization;

namespace DotNessus.Replies
{

    [DataContract(Name = "scan", Namespace = "")]
    internal class ScanItem
    {
        [DataMember(Name = "uuid")]
        public string Uuid { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "start_time")]
        public double StartTime { get; set; }

        [DataMember(Name = "scan_name")]
        public string ScanName { get; set; }
    }
}

