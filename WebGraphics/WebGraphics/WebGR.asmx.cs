using System.Web.Services;
using System.Xml;
using WebGraphics.Models;

namespace WebGraphics
{
    /// <summary>
    /// Summary description for WebGR
    /// </summary>
    [WebService(Namespace = "webgraphics")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebGR : WebService
    {

        [WebMethod]
        public string GetImage(GraphicsData gdata)
        {
            CreateImage res = new CreateImage(new CreatePicture(gdata).MakeGraph());
            return res.ImageString;
        }

        [WebMethod]
        public XmlElement GetSVGImage(GraphicsData gdata)
        {
            return new CreatePicture(gdata).MakeSVG();
        }
    }
}
