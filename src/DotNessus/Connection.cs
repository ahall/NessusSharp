using System;
using System.Xml;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;

namespace DotNessus
{
    /// <summary>
    /// A connection to a Nessus server.
    /// </summary>
    public class Connection : IConnection
    {
        /// <summary>
        /// Hostname of the server.
        /// </summary>
        private string hostname;

        private string username;
        private string password;
        private string baseUrl;

        private static readonly string BASEURL_TEMPLATE = "https://{0}:8834";

        // The login token.
        private string accessToken;

        /// <summary>
        /// Constructs a Connection and logins immediately.
        /// </summary>
        /// <param name="username">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="password">
        /// A <see cref="System.String"/>
        /// </param>
        public Connection(string username, string password)
            : this("localhost", username, password)
        {
        }

        public Connection(string hostname, string username, string password)
        {
            this.hostname = hostname;
            this.username = username;
            this.password = password;
            this.baseUrl = string.Format(BASEURL_TEMPLATE, this.hostname);

            this.Login();
        }

        private string BuildUrl(params string[] components)
        {
            string suffix = string.Join("/", components);
            return string.Format("{0}/{1}", baseUrl, suffix);
        }

        public List<Policy> ListPolicies()
        {
            List<Policy> policies = new List<Policy>();

            string url = BuildUrl("policy", "list");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("token", accessToken);

            string response = request.GetResponse();
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
            string url = BuildUrl("policy", "add");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("token", accessToken);

            // Nessus XMLrpc states it wants an ID of 0 when creating.
            request.Add("policy_id", Policy.INVALID_ID.ToString());
            request.Add("policy_name", policy.Name);

            // Now copy all the params from the policy into the request.
            foreach (var parm in policy.Parameters)
            {
                request.Add(parm.Key, parm.Value);
            }

            string response = request.GetResponse();
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
            string url = BuildUrl("policy", "delete");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("token", accessToken);

            // Nessus XMLrpc states it wants an ID of 0 when creating.
            request.Add("policy_id", policy.Id.ToString());
            request.Add("policy_name", policy.Name);

            // Now copy all the params from the policy into the request.
            foreach (var parm in policy.Parameters)
            {
                request.Add(parm.Key, parm.Value);
            }

            string response = request.GetResponse();
            Replies.PolicyDeleteReply reply = GetReply<Replies.PolicyDeleteReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            policy.Id = Policy.INVALID_ID;
        }

        public void CreateScan(Scan scan, Policy policy)
        {
            string url = BuildUrl("scan", "new");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("token", accessToken);

            request.Add("scan_name", scan.Name);
            request.Add("target", scan.EncodeTargets());
            request.Add("policy_id", policy.Id.ToString());
            string response = request.GetResponse();

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

        /// <summary>
        /// Deserializes a reply from the response.
        /// </summary>
        /// <param name="response">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        private T GetReply<T>(string response)
        {
            using (XmlDictionaryReader reader =
                   XmlDictionaryReader.CreateTextReader(Encoding.ASCII.GetBytes(response),
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
        private void Login()
        {
            string url = BuildUrl("login");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("login", username);
            request.Add("password", password);
            string response = request.GetResponse();

            Replies.LoginReply reply = GetReply<Replies.LoginReply>(response);
            if (reply.Status != "OK")
            {
                throw new Exception("Login reply failed");
            }

            accessToken = reply.Contents.Token;
        }
    }
}

