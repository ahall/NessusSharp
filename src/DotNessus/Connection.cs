using System;
using System.Xml;
using System.Runtime.Serialization;
using System.Text;

namespace DotNessus
{


    /// <summary>
    /// A connection to a Nessus server.
    /// </summary>
    public class Connection
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
            : this("localhost", username, password, true)
        {
        }

        public Connection(string username, string password, bool loginNow)
            : this("localhost", username, password, loginNow)
        {
        }

        public Connection(string hostname, string username, string password, bool loginNow)
        {
            this.hostname = hostname;
            this.username = username;
            this.password = password;
            this.baseUrl = string.Format(BASEURL_TEMPLATE, hostname);

            if (loginNow)
            {
                this.Login();
            }
        }

        private string buildUrl(params string[] components)
        {
            string suffix = string.Join("/", components);
            return string.Format("{0}/{1}", baseUrl, suffix);
        }

        /// <summary>
        /// Creates a new Nessus Policy on the backend.
        /// </summary>
        public void CreatePolicy(Policy policy)
        {
            string url = buildUrl("policy", "add");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("token", accessToken);

            // Nessus XMLrpc states it wants an ID of 0 when creating.
            request.Add("policy_id", "0");
            request.Add("policy_name", policy.Name);

            // Now copy all the params from the policy into the request.
            foreach (var parm in policy.Parameters)
            {
                request.Add(parm.Key, parm.Value);
            }

            string response = request.GetResponse();

            using (XmlDictionaryReader reader =
                   XmlDictionaryReader.CreateTextReader(Encoding.ASCII.GetBytes(response),
                                                        new XmlDictionaryReaderQuotas()))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Replies.PolicyAddReply));

                // Deserialize the data and read it from the instance.
                Replies.PolicyAddReply reply = (Replies.PolicyAddReply)ser.ReadObject(reader, true);
                if (reply.Status != "OK")
                {
                    throw new Exception("Login reply failed");
                }
            }
        }

        /// <summary>
        /// Connects to the server and attempts to log in.
        /// </summary>
        public void Login()
        {
            string url = buildUrl("login");
            WebPostRequest request = new WebPostRequest(url);
            request.Add("login", username);
            request.Add("password", password);
            string response = request.GetResponse();

            using (XmlDictionaryReader reader =
                   XmlDictionaryReader.CreateTextReader(Encoding.ASCII.GetBytes(response),
                                                        new XmlDictionaryReaderQuotas()))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Replies.LoginReply));

                // Deserialize the data and read it from the instance.
                Replies.LoginReply reply = (Replies.LoginReply)ser.ReadObject(reader, true);
                if (reply.Status != "OK")
                {
                    throw new Exception("Login reply failed");
                }

                accessToken = reply.Contents.Token;
            }
        }
    }
}

