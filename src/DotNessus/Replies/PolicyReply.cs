using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace DotNessus.Replies
{
    [DataContract(Name = "reply", Namespace = "")]
    public class PolicyReply
    {
        [DataContract(Name = "policy", Namespace = "")]
        public class PolicyItem
        {
            [DataMember(Name = "policyID")]
            public int PolicyID { get; set; }

            [DataMember(Name = "policyName")]
            public string PolicyName { get; set; }

            [DataMember(Name = "policyOwner")]
            public string PolicyOwner { get; set; }

            [DataMember(Name = "visibility")]
            public string Visibility { get; set; }
        }

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

        public PolicyReply ()
        {
        }
    }
}

