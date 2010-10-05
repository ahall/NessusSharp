using System;
using System.Runtime.Serialization;

namespace DotNessus.Replies
{

    [DataContract(Name = "reply", Namespace = "")]
    internal class PolicyDeleteReply
    {

        /// <summary>
        /// The contents of the reply.
        /// </summary>
        [DataContract(Name = "contents", Namespace = "")]
        public class ReplyContents
        {
            [DataMember(Name = "policyID")]
            public int PolicyID { get; set; }
        }

        [DataMember(Name = "seq")]
        public string Seq { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "contents")]
        public ReplyContents Contents { get; set; }

        public PolicyDeleteReply()
        {
        }
    }
}

