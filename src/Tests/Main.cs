using System;
using System.Linq;
using DotNessus;
using System.Collections.Generic;

namespace Tests
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            IConnection conn = new Connection("ahall", "temp123");

            Policy policy = new Policy("ahallpolicy");
            policy.AddSmbCredentials("smbuser", "smbpass");
            policy.AddSshCredentials("sshuser", "sshpass");

            conn.CreatePolicy(policy);

            Scan scan = new Scan("fishers");
            conn.CreateScan(scan, policy);

            // Wipes all policies.
            conn.ListPolicies().ForEach(x => conn.DeletePolicy(x));
        }
    }
}

