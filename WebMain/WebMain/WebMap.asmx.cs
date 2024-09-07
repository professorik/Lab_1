using System.Web.Services;
using WebMain.Models;

namespace WebMain
{
    /// <summary>
    /// Summary description for WebMap
    /// </summary>
    [WebService(Namespace = "map")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebMap : System.Web.Services.WebService
    {

        [WebMethod]
        public ResponseData GetData(RequestData client_data)
        {
            ServiceGeometry.WebGeometryServiceSoapClient geom_client = new ServiceGeometry.WebGeometryServiceSoapClient();
            ServiceGeometry.InputData sdata = new ServiceGeometry.InputData();
            sdata.Lines = client_data.Data.Lines;
            ServiceGeometry.OutputData interpoint = geom_client.Intersec(sdata);

            ServiceGraphics.GraphicsData gdata = new ServiceGraphics.GraphicsData();
            gdata.Lines = new ServiceGraphics.Line[sdata.Lines.Length];
            for (int i = 0; i < sdata.Lines.Length; ++i)
            {
                gdata.Lines[i] = new ServiceGraphics.Line();
                gdata.Lines[i].A = MapPoint(sdata.Lines[i].A);
                gdata.Lines[i].B = MapPoint(sdata.Lines[i].B);
            }
            gdata.InterPoint = MapPoint(interpoint.Inter);
            gdata.Width = client_data.W;
            gdata.Height = client_data.H;
            ServiceGraphics.WebGRSoapClient graphics_client = new ServiceGraphics.WebGRSoapClient();

            ResponseData result = new ResponseData();
            result.Image = graphics_client.GetImage(gdata);
            result.A = interpoint.Inter.X;
            result.B = interpoint.Inter.Y;
            return result;
        }

        private ServiceGraphics.Point MapPoint(ServiceGeometry.Point point)
        {
            ServiceGraphics.Point result = new ServiceGraphics.Point
            {
                X = point.X,
                Y = point.Y
            };
            return result;
        }
    }
}
