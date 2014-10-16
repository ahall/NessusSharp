using System;
using System.Xml;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace NessusSharp
{
    /// <summary>
    /// A connection to a Nessus server.
    /// </summary>
    public class Connection : IConnection
    {
        private string username;
        private string password;
        private Uri baseUri;

        // The login token.
        private string accessToken;
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private bool authenticated;

        public Connection(Uri baseUri, string username, string password)
        {
            this.baseUri = baseUri;
            this.username = username;
            this.password = password;
            this.authenticated = false;
        }

        public List<Policy> ListPolicies()
        {
            if (!authenticated)
            {
                Login();
            }

            List<Policy> policies = new List<Policy>();

            Uri uri = new Uri(baseUri, "policy/list");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("token", accessToken);

            var response = request.GetResponse();
            Replies.PolicyListReply reply = GetReply<Replies.PolicyListReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            foreach (Replies.PolicyItem polItem in reply.Contents.Policies)
            {
                policies.Add(new Policy(polItem.PolicyID, polItem.PolicyName));
            }

            return policies;
        }

        public void CreatePolicy(Policy policy)
        {
            if (!authenticated)
            {
                Login();
            }

            Uri uri = new Uri(baseUri, "policy/add");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("token", accessToken);

            // Nessus XMLrpc states it wants an ID of 0 when creating.
            request.Add("policy_id", Policy.INVALID_ID.ToString());
            request.Add("policy_name", policy.Name);
            request.Add("policy_shared", "0");

            // Now copy all the params from the policy into the request.
            foreach (var parm in policy.Parameters)
            {
                request.Add(parm.Key, parm.Value);
            }

            var response = request.GetResponse();
            Replies.PolicyAddReply reply = GetReply<Replies.PolicyAddReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            policy.Id = reply.Contents.PolicyItem.PolicyID;
            policy.Name = reply.Contents.PolicyItem.PolicyName;
        }

        /// <summary>
        /// Deletes a policy from the backend.
        /// </summary>
        /// <param name="policy">
        /// A <see cref="Policy"/>
        /// </param>
        public void DeletePolicy(Policy policy)
        {
            if (!authenticated)
            {
                Login();
            }

            Uri uri = new Uri(baseUri, "policy/delete");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("token", accessToken);

            // Nessus XMLrpc states it wants an ID of 0 when creating.
            request.Add("policy_id", policy.Id.ToString());
            request.Add("policy_name", policy.Name);

            // Now copy all the params from the policy into the request.
            foreach (var parm in policy.Parameters)
            {
                request.Add(parm.Key, parm.Value);
            }

            var response = request.GetResponse();
            Replies.PolicyDeleteReply reply = GetReply<Replies.PolicyDeleteReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            policy.Id = Policy.INVALID_ID;
        }

        public void CreateScan(Scan scan, Policy policy)
        {
            if (!authenticated)
            {
                Login();
            }

            Uri uri = new Uri(baseUri, "scan/new");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("token", accessToken);

            request.Add("scan_name", scan.Name);
            request.Add("target", scan.EncodeTargets());
            request.Add("policy_id", policy.Id.ToString());
            HttpWebResponse response = request.GetResponse();

            Replies.ScanNewReply reply = GetReply<Replies.ScanNewReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            scan.Uuid = reply.Contents.ScanItem.Uuid;

            // Start time is given in UTC seconds so we must add it up and store it in UTC.
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            scan.StartTime = epoch.AddSeconds(reply.Contents.ScanItem.StartTime);
        }

        public List<Report> ListReports()
        {
            if (!authenticated)
            {
                Login();
            }

            List<Report> reports = new List<Report>();

            Uri uri = new Uri(baseUri, "report/list");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("token", accessToken);

            HttpWebResponse response = request.GetResponse();
            Replies.ReportListReply reply = GetReply<Replies.ReportListReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("ReportList reply failed");
            }

            foreach (Replies.ReportItem reportItem in reply.Contents.Reports)
            {
                Report.ScanStatus scanStatus = (Report.ScanStatus)Enum.Parse(typeof(Report.ScanStatus), reportItem.Status, true);
                DateTime timeStamp = EPOCH.AddSeconds(reportItem.TimeStamp);
                reports.Add(new Report(timeStamp, scanStatus, reportItem.Name));
            }

            return reports;
        }

        public void DownloadReport(string name, Stream outStream)
        {
            if (!authenticated)
            {
                Login();
            }

            Uri uri = new Uri(baseUri, "file/report/download");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("token", accessToken);
            request.AddIfNotEmpty("report", name);

            HttpWebResponse response = request.GetResponse();
            Stream inStream = response.GetResponseStream();

            byte[] buffer = new byte[4096];
            while (true)
            {
                int bytesRead = inStream.Read(buffer, 0, 4096);
                if (bytesRead == 0)
                {
                    break;
                }

                outStream.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Deserializes a reply from the response.
        /// </summary>
        /// <param name="response">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        private T GetReply<T>(HttpWebResponse response)
        {
            string strResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            using (XmlDictionaryReader reader =
                   XmlDictionaryReader.CreateTextReader(Encoding.ASCII.GetBytes(strResponse),
                                                        new XmlDictionaryReaderQuotas()))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));

                // Deserialize the data and read it from the instance.
                return (T)ser.ReadObject(reader, true);
            }
        }

        /// <summary>
        /// Connects to the server and attempts to log in.
        /// </summary>
        public void Login()
        {
            Uri uri = new Uri(baseUri, "login");
            WebPostRequest request = new WebPostRequest(uri);
            request.Add("login", username);
            request.Add("password", password);
            HttpWebResponse response = request.GetResponse();

            Replies.LoginReply reply = GetReply<Replies.LoginReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            accessToken = reply.Contents.Token;
            authenticated = true;
        }
    }
}

