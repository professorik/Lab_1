namespace WebMain.Models
{
    public class RequestData
    {
        public int W {  get; set; }
        public int H { get; set; }
        public ServiceGeometry.InputData Data { get; set; }

        public bool SVG { get; set; }
    }
}