using System.Collections.Generic;

namespace WebMain.Models
{
    public class GraphicsData
    {
        public GraphicsData()
        {
            Lines = new List<Line>();
            InterPoint = new Point();
        }

        public List<Line> Lines { get; set; }
        public Point InterPoint { get; set; }
        public string Error { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}