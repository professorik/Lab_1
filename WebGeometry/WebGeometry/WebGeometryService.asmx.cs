using System;
using System.Web.Services;
using WebGeometry.Models;

namespace WebGeometry
{
    /// <summary>
    /// Summary description for WebGeometryService
    /// </summary>
    [WebService(Namespace = "webgeom")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebGeometryService : WebService
    {
        Geometry _geom = new Geometry();

        [WebMethod]
        public OutputData Intersec(InputData data)
        {
            try
            {
                Point X = _geom.Intersec(data);
                return new OutputData(X);
            }
            catch (Exception e)
            {
                return new OutputData("Invalid data: " + e.Message);
            }
        }
    }
}
