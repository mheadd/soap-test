using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using SoapHttpClient;
using SoapHttpClient.Extensions;

namespace soap_test
{
    class Program
    {
        private static XNamespace ns = XNamespace.Get("http://helio.spdf.gsfc.nasa.gov/");
        private static string endpoint = "http://sscweb.gsfc.nasa.gov:80/WS/helio/1/HeliocentricTrajectoriesService";
        private static SoapClient soapClient = new SoapClient();
        static void Main(string[] args)
        {
            XElement body = new XElement(ns.GetName("getAllObjects"));
            using (soapClient)
            {
                try
                {
                    var result = soapClient.Post(
                          endpoint: endpoint,
                          body: body);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(JsonConvert.SerializeXmlNode(doc));
                }
                catch (XmlException)
                {
                    Console.WriteLine("ERROR: Unable to get SOAP response");
                }
                catch (Exception exception) {
                    Console.WriteLine($"ERROR: {exception.Message}");
                }
            }
        }
    }
    public class ContentTypeChangingHandler : DelegatingHandler
    {
        public ContentTypeChangingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected async override Task<HttpResponseMessage> SendAsync(
          HttpRequestMessage request,
          CancellationToken cancellationToken)
        {

            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
