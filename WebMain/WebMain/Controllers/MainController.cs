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
using System.Xml;
using WebMain.Models;
using WebMain.Handlers;

namespace WebMain.Controllers
{
    public class MainController : ApiController
    {

        [Route("calculate")]
        [HttpPost]
        public IHttpActionResult GetData([FromBody] RequestData client_data)
        {
            try
            {
                ResponseData result = new ResponseData();
                
                (Point, string) response = GeometryServiceHandler.getInterPoint(client_data);
                if (response.Item1 != null)
                {
                    result.A = response.Item1.X;
                    result.B = response.Item1.Y;
                }
                result.Error = response.Item2;

                (string, XmlElement) imageResponse = GraphicServiceHandler.createImage(client_data, response.Item1, response.Item2);
                result.Image = imageResponse.Item1;
                result.SvgImage = imageResponse.Item2;
                return Ok(result);
            }
            catch (Exception e) {
                return Ok(e.Message);
            }
        }
    }
}