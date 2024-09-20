using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using WebMain.Models;

namespace WebMain.Controllers
{
    public class MainController : ApiController
    {

        private const string geomUri = "http://localhost:49763/api/";
        private const string graphicsUri = "http://localhost:62547/";


        [Route("calculate")]
        [HttpGet]
        //[HttpPost]
        public IHttpActionResult GetData([FromUri] RequestData client_data)
        {
            try
            {
                ResponseData result = new ResponseData();
                
                (Point, string) response = getInterPoint(client_data);
                if (response.Item1 != null)
                {
                    result.A = response.Item1.X;
                    result.B = response.Item1.Y;
                }
                result.Error = response.Item2;

                (string, XmlElement) imageResponse = createImage(client_data, response.Item1, response.Item2);
                result.Image = imageResponse.Item1;
                result.SvgImage = imageResponse.Item2;

                return Ok(result);
            }
            catch (Exception e) {
                return Ok(e.Message);
            }
        }

        [NonAction]
        private (string, XmlElement) createImage(RequestData client_data, Point interpoint, string error)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(graphicsUri);

            GraphicsData gd = new GraphicsData();
            gd.Width = client_data.W;
            gd.Height = client_data.H;
            gd.Lines = client_data.Lines;
            gd.InterPoint = interpoint == null? new Point(): interpoint;
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
            return (imageResponse.Content.ReadAsStringAsync().Result, null);
        }


        [NonAction]
        private (Point, string) getInterPoint(RequestData client_data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(geomUri);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            Dictionary<string, float> _params = new Dictionary<string, float>();
            for (int i = 0; i < 2; i++)
            {
                _params.Add(string.Format("[{0}].A.X", i), client_data.Lines[i].A.X);
                _params.Add(string.Format("[{0}].A.Y", i), client_data.Lines[i].A.Y);
                _params.Add(string.Format("[{0}].B.X", i), client_data.Lines[i].B.X);
                _params.Add(string.Format("[{0}].B.Y", i), client_data.Lines[i].B.Y);
            }
            string paramStr = string.Join("&", _params.Select(p => string.Format("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value.ToString(CultureInfo.InvariantCulture)))));
            string requestUri = string.Format("geometry?{0}", paramStr);

            HttpResponseMessage response = client.GetAsync(requestUri).Result;
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            if (response.IsSuccessStatusCode)
            {
                Point interPoint = json_serializer.Deserialize<Point>(response.Content.ReadAsStringAsync().Result);
                return (interPoint, String.Empty);
            }
            string errorMsg = response.StatusCode == HttpStatusCode.BadRequest ? 
                json_serializer.Deserialize<ErrorMsg>(response.Content.ReadAsStringAsync().Result).Message : "something went wrong";
            return (null, errorMsg);
        }
    }
}
