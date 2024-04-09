using System;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using RestSharp;
using System.Net;

namespace FcsSampleRequest
{

    public class SendToMSB
    {

        public ResponseReq SendRequest(string url)
        {
            ResponseReq res = new ResponseReq(false, null);
            try
            {
                FcsSampleRequest.WebMBriut.WSMisradHabriut WsM = new FcsSampleRequest.WebMBriut.WSMisradHabriut();
                //WsM.HelloWorld();

                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);


                //var WSresponse = WsM.FcsSampleRequest(barcode);

                //string result = misBrWS.FcsSampleRequest("1");
                //FCS_OBJECTS.ResponseServise result = misBrWS. FcsSampleRequest("1");
                IRestResponse response = client.Execute(request);
                //var WSresponse = client.Execute(request);
                var WSresponse = response.Content;
                if (WSresponse != "") {
                    string[] resError = WSresponse.ToString().Split(';');
                    //res.Msg = "resError[0]-" + resError[0] + "-resError[1]-" + resError[1];
                    res.Msg = resError[0] + ";" + resError[2];
                    res.success = resError[1] == "True" ? true : false;

                }
                else
                {
                    res.Msg = "";
                    res.success = false;
                }

                return res;
            }
            catch (Exception e)
            {
                //throw the exeption
                Console.WriteLine("Err at request: " + e.Message + "- " + e.InnerException);
                res.Msg = "Exception-" + e.Message + "- "+ e.InnerException;

                res.success = false;
                return res;
            }


        }
    }
}




    //< appSettings >

    //    < add key = "eventSourceName" value = "MySource" />
   
    //       < add key = "logName" value = "C:\Work\log" />
      
    //          < add key = "LogPath" value = "C:\Work\log2" />
         
    //             < add key = "QueriesPath" value = "C:\Work\log3" />
            
    //            </ appSettings >





