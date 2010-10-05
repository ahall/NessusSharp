using System;
using System.Runtime.Serialization;

namespace NessusSharp.Replies
{
    [DataContract(Name = "policy", Namespace = "")]
    internal class PolicyItem
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

}

