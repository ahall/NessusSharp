using System;
using System.Net;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace NessusSharp
{
    public class WebPostRequest
    {
        private HttpWebRequest request;
        private List<string> queryData;

        public WebPostRequest(Uri uri) : this(uri, new CookieContainer())
        {
        }

        public WebPostRequest(Uri uri, CookieContainer cookies)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Method = "POST";
            queryData = new List<string>();
        }

        private bool AcceptAllCertifications(object sender,
                                             System.Security.Cryptography.X509Certificates.X509Certificate certification,
                                             System.Security.Cryptography.X509Certificates.X509Chain chain,
                                             System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Add(string key, string value)
        {
            queryData.Add(string.Format("{0}={1}", key, HttpUtility.UrlEncode(value)));
        }

        public void AddIfNotEmpty(string key, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                Add(key, val);
            }
        }

        /// <summary>
        /// Sends the request and retrieves a response.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public HttpWebResponse GetResponse()
        {
            // Set the encoding type
            request.ContentType = "application/x-www-form-urlencoded";

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
            return request.GetResponse() as HttpWebResponse;
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
