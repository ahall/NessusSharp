using System;
using System.Linq;
using NessusSharp;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Uri baseUri = new Uri("https://localhost:8834");
            IConnection conn = new Connection(baseUri, "ahall", "temp123");

            List<Report> reports = conn.ListReports();
            foreach (Report report in reports)
            {
                Console.WriteLine(report.Name);
                Console.WriteLine(report.Status);
                Console.WriteLine(report.TimeStamp);
            }

            using (FileStream toWrite = File.OpenWrite("/tmp/writeit.xml"))
            {
                conn.DownloadReport(reports[0].Name, toWrite);
            }

            //Policy policy = new Policy("ahallpolicy");
            //policy.AddSmbCredentials("smbuser", "smbpass");
            //policy.AddSshCredentials("sshuser", "sshpass");

            //conn.CreatePolicy(policy);

            /*
            Scan scan = new Scan("fishers");
            scan.Targets.Add("127.0.0.1");
            scan.Targets.Add("127.0.0.2");
            conn.CreateScan(scan, policy);
            */

            // Wipes all policies.
            //conn.ListPolicies().ForEach(x => conn.DeletePolicy(x));
        }
    }
}

