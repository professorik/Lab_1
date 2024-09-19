using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebMain.Controllers
{
    public class MainController : ApiController
    {
        /*
         * [WebMethod]
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
            gdata.Error = interpoint.Error;
            gdata.InterPoint = MapPoint(interpoint.Inter);
            gdata.Width = client_data.W;
            gdata.Height = client_data.H;
            ServiceGraphics.WebGRSoapClient graphics_client = new ServiceGraphics.WebGRSoapClient();

            ResponseData result = new ResponseData();
            result.Error = interpoint.Error;
            if (client_data.SVG)
            {
                var doc = new XmlDocument();
                doc.Load(graphics_client.GetSVGImage(gdata).CreateReader());
                result.SvgImage = doc.DocumentElement;
                result.Image = "";
            }
            else
            {
                result.Image = graphics_client.GetImage(gdata);
            }
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
         */
    }
}
