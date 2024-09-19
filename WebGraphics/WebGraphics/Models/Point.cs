namespace WebGraphics.Models
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Point()
        {
            X = Y = 0;
        }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}