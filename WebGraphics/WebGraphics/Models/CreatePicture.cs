using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using WebGraphics.WebGeometry;

namespace WebGraphics.Models
{
    public class CreatePicture
    {
        private const float Margin = 5f;
        private float _xmin, _xmax, _ymin, _ymax;
        private float _factor = 1f;
        private GraphicsData _expdata;

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
        }

        public Bitmap MakeGraph()
        {
            Bitmap bm = new Bitmap(_expdata.Width, _expdata.Height);
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                RectangleF rect = new RectangleF(_xmin, _ymin, _xmax - _xmin, _ymax - _ymin);
                PointF[] pts =
                {
                    new PointF(0, _expdata.Height),
                    new PointF(_expdata.Width, _expdata.Height),
                    new PointF(0, 0),
                };
                g.Transform = new Matrix(rect, pts);
                
                DrawAxes(g);
                _expdata.Lines.ForEach(l => DrawLine(g, l));
                if (string.IsNullOrEmpty(_expdata.Error))
                {
                    g.FillEllipse(new SolidBrush(Color.Blue), _expdata.InterPoint.X - _factor / 2, _expdata.InterPoint.Y - _factor / 2, _factor, _factor);
                }
                else if (_expdata.Error.EndsWith("coincident"))
                {
                    (PointF first, PointF second) = GetEdges(_expdata.Lines[0]);
                    g.DrawLine(new Pen(Color.Blue, 0), first, second);
                }
                return bm;
            }
        }

        private void DrawLine(Graphics g, Line l)
        {
            (PointF first, PointF second) = GetEdges(l);
            g.DrawLine(new Pen(Color.Black, 0), first, second);
            g.FillEllipse(new SolidBrush(Color.Green), l.A.X - _factor / 2, l.A.Y - _factor / 2, _factor, _factor);
            g.FillEllipse(new SolidBrush(Color.Red), l.B.X - _factor / 2, l.B.Y - _factor / 2, _factor, _factor);
        }

        private (PointF first, PointF second) GetEdges(Line l)
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

        private void DrawAxes(Graphics g)
        {
            Pen gpen = new Pen(Color.Black, 0);
            g.DrawLine(gpen, 0, _ymin, 0, _ymax);
            g.DrawLine(gpen, _xmin, 0, _xmax, 0);

            float hx = (_xmax - _xmin) / 10f;
            for (float xs = _xmin + hx; xs <= _xmax; xs += hx)
                g.DrawLine(gpen, xs, -0.5f, xs, 0.5f);

            float hy = (_ymax - _ymin) / 10f;
            for (float ys = _ymin + hy; ys <= _ymax; ys += hy)
                g.DrawLine(gpen, -0.5f, ys, 0.5f, ys);
        }
    }
}