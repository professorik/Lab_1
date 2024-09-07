using System;

namespace WebGeometry.Models
{
    public class Geometry
    {
        public Point Intersec(InputData data)
        {
            if (data == null || data.Lines.Count != 2)
            {
                throw new Exception("two lines are required");
            }
            Line line_1 = data.Lines[0];
            Line line_2 = data.Lines[1];
            float ax = line_1.A.X - line_1.B.X;
            float bx = line_2.A.X - line_2.B.X;
            float ay = line_1.A.Y - line_1.B.Y;
            float by = line_2.A.Y - line_2.B.Y;
            float denominator = ax * by - ay * bx;
            if (Math.Abs(denominator) < 1e-5)
            {
                throw new Exception("the two lines are parallel or coincident");
            }
            float tmp_1 = (line_1.A.X * line_1.B.Y - line_1.A.Y * line_1.B.X);
            float tmp_2 = (line_2.A.X * line_2.B.Y - line_2.A.Y * line_2.B.X);
            float P_x = (tmp_1 * bx - tmp_2 * ax) / denominator;
            float P_y = (tmp_1 * by - tmp_2 * ay) / denominator;
            return new Point(P_x, P_y);
        }
    }
}