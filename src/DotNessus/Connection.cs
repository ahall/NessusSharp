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
        private string server;
        private string username;
        private string password;

        public Connection(string server, string username, string password, bool loginNow)
        {
            this.server = server;
            this.username = username;
            this.password = password;

            if (loginNow)
            {
                this.Login();
            }
        }

        /// <summary>
        /// Connects to the server and attempts to log in.
        /// </summary>
        private void Login()
        {
            string url = string.Format("https://{0}:8834/login", server);
            WebPostRequest request = new WebPostRequest(url);
            request.Add("login", username);
            request.Add("password", password);
            string response = request.GetResponse();

            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(Encoding.ASCII.GetBytes(response), new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(Replies.LoginReply));

            // Deserialize the data and read it from the instance.
            Replies.LoginReply reply = (Replies.LoginReply)ser.ReadObject(reader, true);
            reader.Close();

            Console.WriteLine(response);
        }
    }
}

