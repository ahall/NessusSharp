using System;
using System.Runtime.Serialization;

namespace NessusSharp.Replies
{
    [DataContract(Name = "reply", Namespace = "")]
    internal class ScanNewReply
    {

        [DataContract(Name = "contents", Namespace = "")]
        public class ReplyContents
        {
            [DataMember(Name = "scan")]
            public ScanItem ScanItem { get; set; }
        }

        [DataMember(Name = "seq")]
        public string Seq { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "contents")]
        public ReplyContents Contents { get; set; }

        public ScanNewReply ()
        {
        }
    }
}

