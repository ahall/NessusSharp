using System;
using DotNessus;

namespace Tests
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Connection conn = new Connection("ahall", "temp123");

            Policy policy = new Policy("ahallpolicy");
            //policy.AddSmbCredentials("smbuser", "smbpass");
            //policy.AddSshCredentials("sshuser", "sshpass");

            conn.CreatePolicy(policy);
        }
    }
}

