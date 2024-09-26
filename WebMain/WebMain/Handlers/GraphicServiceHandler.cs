using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml;
using WebMain.Models;

namespace WebMain.Handlers
{
    public class GraphicServiceHandler
    {
        private const string _Uri = "http://localhost:62547/";

        public static (string, XmlElement) createImage(RequestData client_data, Point interpoint, string error)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_Uri);

            GraphicsData gd = new GraphicsData();
            gd.Width = client_data.W;
            gd.Height = client_data.H;
            gd.Lines = client_data.Lines;
            gd.InterPoint = interpoint == null ? new Point() : interpoint;
            gd.Error = error;

            var data = new StringContent(JsonConvert.SerializeObject(gd), Encoding.UTF8, "application/json");

            HttpResponseMessage imageResponse = httpClient.PostAsync(client_data.SVG ? "svg" : "bitmap", data).Result;
            if (client_data.SVG)
            {
                var stream = imageResponse.Content.ReadAsStreamAsync().Result;
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);
                return (String.Empty, xmlDocument.DocumentElement);
            }
            string deserializedString = JsonConvert.DeserializeObject<string>(imageResponse.Content.ReadAsStringAsync().Result);
            return (deserializedString, null);
        }
    }
}