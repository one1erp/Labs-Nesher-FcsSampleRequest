using System;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using RestSharp;
using System.Net;

namespace FcsSampleRequest
{
    public class responseReq
    {
        public bool success { get; set; }
        public string strXml { get; set; }
        public int returnCode { get; set; }
        public string returnCodeDesc { get; set; }

        public responseReq(bool success, string strXml, int returnCode, string returnCodeDesc)
        {
            this.success = success;
            this.strXml = strXml;
            this.returnCode = returnCode;
            this.returnCodeDesc = returnCodeDesc;
        }
    }


    class OutgoingReq
    {

        public static responseReq PostReq(string url, string pfxFile, string xmlFile, string SOAPAction)
        {
            responseReq res = new responseReq(false, null,0,null);

            try
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("SOAPAction", SOAPAction);
                request.AddHeader("Content-Type", "text/xml");

                X509Certificate2Collection collection = new X509Certificate2Collection();
                collection.Import(pfxFile, "12345678", X509KeyStorageFlags.PersistKeySet);
                client.ClientCertificates = new X509CertificateCollection();
                client.ClientCertificates.AddRange(collection);
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00); //SecurityProtocolType.Tls;//| SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12

                XmlDocument xdoc = new XmlDocument();//xml doc used for xml parsing
                xdoc.Load(xmlFile);
              
                request.AddParameter("text/xml",xdoc.InnerXml , ParameterType.RequestBody);  //XmlText.bodyLab_Sample_Form
                IRestResponse response = client.Execute(request);
                //Console.WriteLine(response.Content);
                if (response != null && (int)response.StatusCode == 200)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(response.Content);
                    XmlNode Return_Code = xml.GetElementsByTagName("a:Return_Code")[0];
                    res.returnCode = int.Parse(Return_Code.InnerText);

                    if (Return_Code.InnerText == "0" || Return_Code.InnerText == "1")
                    {
                        res.success = true;
                        res.strXml = response.Content;
                    }
                    else
                    {
                        XmlNode Return_Code_Desc = xml.GetElementsByTagName("a:Return_Code_Desc")[0];
                        res.returnCodeDesc = System.Net.WebUtility.HtmlDecode(Return_Code_Desc.InnerText);
                        res.strXml = "api שגיאה בנתוני השליחה ל" + ": " + ((int)response.StatusCode).ToString();                     
                    }
                }
                else
                {
                    res.strXml = ((int)response.StatusCode).ToString() + ": " + response.StatusCode.ToString();
                }
                return res;
            }
            catch (Exception e)
            {
                //throw the exeption
                MessageBox.Show("Err at request: " + e.Message);
                return res;
            }


        }
    }
}



     