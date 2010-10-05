using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace NessusSharp.Replies
{
    [DataContract(Name = "reply", Namespace = "")]
    internal class PolicyAddReply
    {
        /// <summary>
        /// The contents of the reply.
        /// </summary>
        [DataContract(Name = "contents", Namespace = "")]
        public class ReplyContents
        {
            [DataMember(Name = "policy")]
            public PolicyItem PolicyItem { get; set; }
        }

        [DataMember(Name = "seq")]
        public string Seq { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "contents")]
        public ReplyContents Contents { get; set; }

        public PolicyAddReply ()
        {
        }
    }
}

