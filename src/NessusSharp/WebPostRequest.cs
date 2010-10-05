using System;
using System.Net;
using System.Collections.Generic;
using System.Web;
using System.IO;
namespace NessusSharp
{
    internal class WebPostRequest
    {
        private WebRequest request;
        private List<string> queryData;

        public WebPostRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            request = WebRequest.Create(url);
            request.Method = "POST";
            queryData = new List<string>();
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Add(string key, string value)
        {
            queryData.Add(String.Format("{0}={1}", key, HttpUtility.UrlEncode(value)));
        }

        /// <summary>
        /// Sends the request and retrieves a response.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string GetResponse()
        {
            // Set the encoding type
            request.ContentType="application/x-www-form-urlencoded";

            // Build a string containing all the parameters
            string parameters = string.Join("&", queryData.ToArray());
            request.ContentLength = parameters.Length;

            // We write the parameters into the request
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(parameters);
                sw.Flush();
            }

            // Execute the query
            var response =  request.GetResponse() as HttpWebResponse;
            StreamReader sr = new StreamReader(response.GetResponseStream());
            return sr.ReadToEnd();
        }

        public override string ToString()
        {
            string ret = "";
            foreach (string qrydata in queryData)
            {
                ret += qrydata + "\n";
            }

            return ret;
        }

    }
}

