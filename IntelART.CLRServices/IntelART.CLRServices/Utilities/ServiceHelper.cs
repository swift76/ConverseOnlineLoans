using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace IntelART.CLRServices
{
    public class ServiceHelper
    {
        public static int QueryTimeout { get; set; }

        public static string GetNodeValue(XmlDocument document, string nodePath)
        {
            string result = string.Empty;
            XmlNode node = document.SelectSingleNode(nodePath);
            if (node != null)
                result = RetrieveValue(node.InnerXml);
            return result;
        }

        public static DateTime GetNORQDateValue(string dateString, DateTime defaultValue)
        {
            DateTime result;
            try
            {
                result = new DateTime(int.Parse(dateString.Substring(0, 4)), int.Parse(dateString.Substring(5, 2)), int.Parse(dateString.Substring(8, 2)));
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }

        public static string GetFormattedXML(string sourceXML)
        {
            string result = sourceXML;
            using (MemoryStream stream = new MemoryStream())
            using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(sourceXML);
                    writer.Formatting = Formatting.Indented;
                    document.WriteContentTo(writer);
                    writer.Flush();
                    stream.Flush();
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                        result = reader.ReadToEnd();
                }
                catch (XmlException)
                {
                }
            }
            return result;
        }

        public static string GenerateUniqueID(byte length)
        {
            string randomNumber = (new Random()).Next().ToString();
            if (randomNumber.Length > length)
                randomNumber = randomNumber.Substring(0, length);
            else if (randomNumber.Length < length)
                randomNumber = randomNumber.PadLeft(length, '0');
            return randomNumber;
        }

        public static string DecorateValue(string value)
        {
            return string.Format("<![CDATA[{0}]]>", value);
        }

        public static string RetrieveValue(string value)
        {
            string output = value.Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty).Trim();
            output = output.Replace("<", "'");
            output = output.Replace(">", "'");
            return output;
        }

        public static decimal RetrieveOptionalAmount(XmlNode node, string nodeName)
        {
            decimal amount = 0;
            XmlNode foundNode = node.SelectSingleNode(nodeName);
            if (foundNode != null)
                decimal.TryParse(RetrieveValue(foundNode.InnerXml), out amount);
            return amount;
        }

        public static XmlDocument CheckACRAResponse(string responseText)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(responseText);
            string responseCode = GetNodeValue(document, "/ROWDATA[@*]/Response");
            if (responseCode.ToUpper() != "OK")
                throw new ApplicationException(GetNodeValue(document, "/ROWDATA[@*]/Error"));
            return document;
        }

        public static DateTime GetACRADateValue(string dateString, DateTime? defaultValue = null)
        {
            if (string.IsNullOrEmpty(dateString) || dateString == "00-00-0000")
                return defaultValue.Value;
            return new DateTime(int.Parse(dateString.Substring(6, 4)), int.Parse(dateString.Substring(3, 2)), int.Parse(dateString.Substring(0, 2)));
        }

        public static DateTime? GetACRANullableDateValue(string dateString)
        {
            if (string.IsNullOrEmpty(dateString) || dateString == "00-00-0000")
                return null;
            return GetACRADateValue(dateString);
        }

        public static string PrepareNORQRequestXML(string method, Dictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            builder.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            builder.AppendFormat("<{0} xmlns=\"http://norq.am/dxchange/2013\">", method);
            foreach (string key in parameters.Keys)
                builder.AppendFormat("<{0}>{1}</{0}>", key, parameters[key]);
            builder.AppendFormat("</{0}>", method);
            builder.Append("</s:Body>");
            builder.Append("</s:Envelope>");
            return builder.ToString();
        }

        public static string GetNORQResponseText(string url, string method, Dictionary<string, string> parameters)
        {
            string responseText;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("SOAPAction", string.Format("http://norq.am/dxchange/2013/INorqDXChangeSoapService/{0}", method));
            request.ContentType = "text/xml; charset=utf-8";
            request.Timeout = 1000 * QueryTimeout;
            request.ReadWriteTimeout = request.Timeout;
            byte[] bytes = Encoding.UTF8.GetBytes(PrepareNORQRequestXML(method, parameters));
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            using (Stream stream = request.GetRequestStream())
                stream.Write(bytes, 0, bytes.Length);
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                responseText = reader.ReadToEnd();
            responseText = responseText.Replace("xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"", string.Empty);
            responseText = responseText.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty);
            responseText = responseText.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
            responseText = responseText.Replace("xmlns=\"http://norq.am/dxchange/2013\"", string.Empty);
            responseText = responseText.Replace("xsi:nil=\"true\"", string.Empty);
            responseText = responseText.Replace("s:", string.Empty);
            responseText = responseText.Replace("_v2018", string.Empty);
            return responseText;
        }

        public static string NORQ_Login(ServiceConfig config)
        {
            Dictionary<string, string> parametersLogin = new Dictionary<string, string>();
            parametersLogin.Add("argLogin", config.UserName);
            parametersLogin.Add("argPassword", config.UserPassword);
            XmlDocument document = CheckNORQResponse(GetNORQResponseText(config.URL, "Login", parametersLogin));
            return GetNORQNodeValue(document, "Login", "LoginResult");
        }

        public static string ACRA_Login(ServiceConfig config, ref string cookie)
        {
            Dictionary<string, string> parametersLogin = new Dictionary<string, string>();
            parametersLogin.Add("User", config.UserName);
            parametersLogin.Add("Password", config.UserPassword);
            XmlDocument document = CheckACRAResponse(GetACRAResponseText(config.URL, "Bank_LogIn", ref cookie, parametersLogin));
            return GetNodeValue(document, "/ROWDATA[@*]/SID");
        }

        public static XmlDocument CheckNORQResponse(string responseText)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(responseText);
            return document;
        }

        public static string GetNORQNodeValue(XmlDocument document, string method, string nodePath)
        {
            return GetNodeValue(document, string.Format("/Envelope/Body/{0}Response/{1}", method, nodePath));
        }

        public static string PrepareACRARequestXML(string method, Dictionary<string, string> parameters, Dictionary<string, string> participientParameters, List<string> persons)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<ROWDATA type=\"{0}\">", method);
            builder.AppendFormat("<ReqID>{0}</ReqID>", DecorateValue(ServiceHelper.GenerateUniqueID(13)));
            foreach (string key in parameters.Keys)
                builder.AppendFormat("<{0}>{1}</{0}>", key, parameters[key]);
            if ((participientParameters != null && participientParameters.Count > 0) || (persons != null && persons.Count > 0))
            {
                builder.Append("<Participient id=\"1\">");
                builder.Append("<KindBorrower>1</KindBorrower>");
                if (persons != null && persons.Count > 0)
                    foreach (string key in persons)
                        builder.AppendFormat("<Person><Identificator>{0}</Identificator></Person>", key);
                else
                    foreach (string key in participientParameters.Keys)
                        builder.AppendFormat("<{0}>{1}</{0}>", key, participientParameters[key]);
                builder.Append("<RequestTarget>1</RequestTarget>");
                builder.Append("<UsageRange>1</UsageRange>");
                builder.Append("</Participient>");
            }
            builder.Append("</ROWDATA>");
            return builder.ToString();
        }

        public static string GetACRAResponseText(string url, string method, ref string cookie, Dictionary<string, string> parameters, Dictionary<string, string> participientParameters = null, List<string> persons = null)
        {
            string responseText;
            byte[] bytes = Encoding.UTF8.GetBytes(string.Format("query_xml={0}", Uri.EscapeDataString(PrepareACRARequestXML(method, parameters, participientParameters, persons))));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 1000 * QueryTimeout;
            request.ReadWriteTimeout = request.Timeout;
            request.ContentLength = bytes.Length;
            request.Method = "POST";

            if (cookie != null)
                request.Headers.Add("Cookie", cookie.Split(';')[0]);

            using (Stream stream = request.GetRequestStream())
                stream.Write(bytes, 0, bytes.Length);
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                responseText = reader.ReadToEnd();

            if (cookie == null)
                cookie = response.Headers["Set-Cookie"];

            return responseText;
        }

        public static int RetrieveOptionalCount(XmlNode node, string nodeName)
        {
            int count = 0;
            XmlNode foundNode = node.SelectSingleNode(nodeName);
            if (foundNode != null)
                int.TryParse(RetrieveValue(foundNode.InnerXml), out count);
            return count;
        }
    }
}
