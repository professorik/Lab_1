using System.Collections.Generic;

namespace WebGraphics.Models
{
    public class GraphicsData
    {
        public GraphicsData()
        {
            Lines = new List<WebGeometry.Line>();
            InterPoint = new WebGeometry.Point();
        }

        public List<WebGeometry.Line> Lines { get; set; }
        public WebGeometry.Point InterPoint { get; set; }
        public string Error { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}