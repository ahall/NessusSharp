using System;
using System.Runtime.Serialization;
namespace DotNessus.Replies
{
    /// <summary>
    /// A reply we get from login in to the nessus interface.
    /// </summary>
    [DataContract(Name = "reply", Namespace = "")]
    public class LoginReply
    {
        [DataContract(Name = "user", Namespace = "")]
        public class User
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "admin")]
            public string Admin { get; set; }

            public User()
            {
            }
        }

        /// <summary>
        /// The contents of the reply.
        /// </summary>
        [DataContract(Name = "contents", Namespace = "")]
        public class ReplyContents
        {
            [DataMember(Name = "token")]
            public string Token { get; set; }

            [DataMember(Name = "user")]
            public User User { get; set; }
        }


        [DataMember(Name = "seq")]
        public string Seq { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "contents")]
        public ReplyContents Contents { get; set; }

        public LoginReply()
        {
        }
    }
}

