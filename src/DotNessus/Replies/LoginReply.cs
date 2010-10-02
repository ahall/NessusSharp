using System;
using System.Runtime.Serialization;
namespace DotNessus.Replies
{
    [DataContract(Name = "reply", Namespace = "")]
    public class LoginReply
    {
        [DataMember(Name = "seq")]
        public string Seq { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        public LoginReply()
        {
        }
    }
}

