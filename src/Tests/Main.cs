using System;
using System.Linq;
using NessusSharp;
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
            scan.Targets.Add("127.0.0.1");
            scan.Targets.Add("127.0.0.2");
            conn.CreateScan(scan, policy);

            // Wipes all policies.
            conn.ListPolicies().ForEach(x => conn.DeletePolicy(x));
        }
    }
}

