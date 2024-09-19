using System.Web.Http;
using System.Xml;
using WebGraphics.Models;
using WebGraphics.Services;

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
        public XmlElement GetSVG([FromBody] GraphicsData data)
        {
            return new CreateSVG(data).MakePicture();
        }
    }
}
