using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using WebMain.Models;

namespace WebMain.Handlers
{
    public class GeometryServiceHandler
    {
        private const string _Uri = "http://localhost:49763/api/";

        public static (Point, string) getInterPoint(RequestData client_data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_Uri);
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
            if (response.IsSuccessStatusCode)
            {
                Point interPoint = JsonConvert.DeserializeObject<Point>(response.Content.ReadAsStringAsync().Result);
                return (interPoint, String.Empty);
            }
            string errorMsg = response.StatusCode == HttpStatusCode.BadRequest ?
                JsonConvert.DeserializeObject<ErrorMsg>(response.Content.ReadAsStringAsync().Result).Message : "something went wrong";
            return (null, errorMsg);
        }
    }
}