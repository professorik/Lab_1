using System.Net.Http;
using System.Net;
using System.Text;
using System.Web.Http;
using System.Xml;
using WebGraphics.Models;
using WebGraphics.Services;
using System.Web.UI.WebControls;

namespace WebGraphics.Controllers
{
    public class GraphicsController : ApiController
    {
        [Route("bitmap")]
        [HttpPost]
        public string GetBitmap([FromBody] GraphicsData data)
        {
            CreateImage res = new CreateImage(new CreateBitMap(data).MakePicture());
            return res.ImageString;
        }

        [Route("svg")]
        [HttpPost]
        public IHttpActionResult GetSVG([FromBody] GraphicsData data)
        {
            XmlElement element = new CreateSVG(data).MakePicture();
            return Content(HttpStatusCode.OK, element, Configuration.Formatters.XmlFormatter);
        }
    }
}