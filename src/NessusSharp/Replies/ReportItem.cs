using System;
using System.Runtime.Serialization;

namespace NessusSharp.Replies
{
    [DataContract(Name = "report", Namespace = "")]
    internal class ReportItem
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "readableName")]
        public string ReadableName { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "timestamp")]
        public double TimeStamp { get; set; }
    }

}


