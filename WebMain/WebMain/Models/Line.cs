namespace WebMain.Models
{
    public class Line
    {
        public Point A { get; set; }
        public Point B { get; set; }

        public Line()
        {
            A = new Point();
            B = new Point();
        }
        public Line(Point _a, Point _b)
        {
            A = _a;
            B = _b;
        }

        public Line(float ax, float ay, float bx, float by)
        {
            A = new Point(ax, ay);
            B = new Point(bx, by);
        }
    }
}