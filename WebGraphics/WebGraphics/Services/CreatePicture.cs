using System;
using System.Drawing;
using WebGraphics.Models;

namespace WebGraphics.Services
{
    public abstract class CreatePicture<T>
    {

        private const float Margin = 5f;

        internal float _xmin, _xmax, _ymin, _ymax;
        internal float _factor = 1f;
        internal GraphicsData _expdata;
        internal RectangleF canvas_rect;
        internal PointF[] canvas_pts;

        public abstract T MakePicture();
        protected abstract void DrawLine(Line l, Color color);
        protected abstract void DrawAxes();

        public CreatePicture(GraphicsData expdata)
        {
            _expdata = expdata;
            float max_value = 2 * Math.Max(Math.Abs(expdata.InterPoint.X), Math.Abs(expdata.InterPoint.Y));
            foreach (Line l in _expdata.Lines)
            {
                max_value = Math.Max(max_value, Math.Abs(l.A.X) + Margin);
                max_value = Math.Max(max_value, Math.Abs(l.B.X) + Margin);
                max_value = Math.Max(max_value, Math.Abs(l.A.Y) + Margin);
                max_value = Math.Max(max_value, Math.Abs(l.B.Y) + Margin);
            }
            _xmax = max_value;
            _ymax = _xmax;
            _xmin = -_xmax;
            _ymin = -_ymax;
            _factor = _xmax / 20f;
            canvas_rect = new RectangleF(_xmin, _ymin, _xmax - _xmin, _ymax - _ymin);
            canvas_pts = new PointF[]{
                new PointF(0, _expdata.Height),
                new PointF(_expdata.Width, _expdata.Height),
                new PointF(0, 0)
            };
        }

        internal (PointF first, PointF second) GetEdges(Line l)
        {
            if (Math.Abs(l.B.X - l.A.X) < 1e-5)
                return (new PointF(l.A.X, _ymin), new PointF(l.B.X, _ymax));
            if (Math.Abs(l.B.Y - l.A.Y) < 1e-5)
                return (new PointF(_xmin, l.A.Y), new PointF(_xmax, l.B.Y));
            float a = (l.B.Y - l.A.Y) / (l.B.X - l.A.X);
            float tmp = l.B.Y + a * (_xmax - l.B.X);
            PointF first = Math.Abs(tmp) <= _ymax ? new PointF(_xmax, tmp) : new PointF(l.B.X + (_ymax - l.B.Y) / a, _ymax);
            tmp = l.A.Y + a * (_xmin - l.A.X);
            PointF second = Math.Abs(tmp) <= _ymax ? new PointF(_xmin, tmp) : new PointF(l.A.X + (_ymin - l.A.Y) / a, _ymin);
            return (first, second);
        }
    }
}