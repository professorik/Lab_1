using System.Collections.Generic;

namespace WebMain.Models
{
    public class RequestData
    {
        public int W { get; set; }
        public int H { get; set; }
        public List<Line> Lines { get; set; }
        public bool SVG { get; set; }
    }
}