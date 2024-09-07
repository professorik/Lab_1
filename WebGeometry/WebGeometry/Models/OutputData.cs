namespace WebGeometry.Models
{
    public class OutputData
    {
        public Point Inter { get; set; }
        public string Error { get; set; }

        public OutputData()
        {
            Inter = new Point();
            Error = string.Empty;
        }
        public OutputData(Point inter)
        {
            Inter = inter;
            Error = string.Empty;
        }

        public OutputData(string error)
        {
            Inter = new Point();
            Error = error;
        }
    }
}