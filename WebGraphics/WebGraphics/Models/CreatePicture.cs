using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Globalization;
using System.Xml;
using WebGraphics.WebGeometry;

namespace WebGraphics.Models
{
    public class CreatePicture
    {
        private const float Margin = 5f;
        private float _xmin, _xmax, _ymin, _ymax;
        private float _factor = 1f;
        private GraphicsData _expdata;
        private RectangleF canvas_rect;
        private PointF[] canvas_pts;

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

        public Bitmap MakeGraph()
        {
            Bitmap bm = new Bitmap(_expdata.Width, _expdata.Height);
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Transform = new Matrix(canvas_rect, canvas_pts);
                
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

        public XmlElement MakeSVG()
        {
            RectangleF rect = new RectangleF(_xmin, _ymin, _xmax - _xmin, _ymax - _ymin);
            PointF[] pts =
            {
                    new PointF(0, _expdata.Height),
                    new PointF(_expdata.Width, _expdata.Height),
                    new PointF(0, 0),
              };

            XmlDocument doc = new XmlDocument();
            XmlElement svg = doc.CreateElement("svg");
            svg.SetAttribute("width", _expdata.Width.ToString());
            svg.SetAttribute("height", _expdata.Height.ToString());
            svg.SetAttribute("viewBox", 
                _xmin.ToString(CultureInfo.InvariantCulture) + " " + 
                _ymin.ToString(CultureInfo.InvariantCulture) + " " +
                (_xmax - _xmin).ToString(CultureInfo.InvariantCulture) + " " +
                (_ymax - _ymin).ToString(CultureInfo.InvariantCulture)
            );
            svg.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            SetSVGMatrix(svg);
            doc.AppendChild(svg);

            DrawAxesSVG(doc, svg);
            _expdata.Lines.ForEach(l => DrawLineSVG(doc, svg, Color.Black, l));
            if (string.IsNullOrEmpty(_expdata.Error))
            {
                svg.AppendChild(GetCircleSVG(doc, Color.Blue, _expdata.InterPoint.X, _expdata.InterPoint.Y, 3));
            }
            else if (_expdata.Error.EndsWith("coincident"))
            {
                (PointF first, PointF second) = GetEdges(_expdata.Lines[0]);
                svg.AppendChild(GetLineSVG(doc, Color.Blue, first, second, true));
            }
            return doc.DocumentElement;
        }

        private void DrawAxesSVG(XmlDocument doc, XmlElement svg)
        {
            svg.AppendChild(GetLineSVG(doc, Color.Black, 0, _ymin, 0, _ymax));
            svg.AppendChild(GetLineSVG(doc, Color.Black, _xmin, 0, _xmax, 0));
            float hx = (_xmax - _xmin) / 10f;
            for (float xs = _xmin + hx; xs <= _xmax; xs += hx)
                svg.AppendChild(GetLineSVG(doc, Color.Black, xs, -1f, xs, 1f));

            float hy = (_ymax - _ymin) / 10f;
            for (float ys = _ymin + hy; ys <= _ymax; ys += hy)
                svg.AppendChild(GetLineSVG(doc, Color.Black, -1f, ys, 1f, ys));
        }

        private void DrawLineSVG(XmlDocument doc, XmlElement svg, Color color, Line l)
        {
            (PointF first, PointF second) = GetEdges(l);
            svg.AppendChild(GetLineSVG(doc, color, first, second));
            svg.AppendChild(GetCircleSVG(doc, Color.Green, l.A.X, l.A.Y, 3));
            svg.AppendChild(GetCircleSVG(doc, Color.Red, l.B.X, l.B.Y, 3));
        }

        private XmlElement GetLineSVG(XmlDocument doc, Color color, PointF a, PointF b, bool dashed = false)
        {
            return GetLineSVG(doc, color, a.X, a.Y, b.X, b.Y, dashed);
        }

        private XmlElement GetLineSVG(XmlDocument doc, Color color, float x1, float y1, float x2, float y2, bool dashed = false)
        {
            XmlElement line = doc.CreateElement("line");
            line.SetAttribute("x1", x1.ToString(CultureInfo.InvariantCulture));
            line.SetAttribute("y1", y1.ToString(CultureInfo.InvariantCulture));
            line.SetAttribute("x2", x2.ToString(CultureInfo.InvariantCulture));
            line.SetAttribute("y2", y2.ToString(CultureInfo.InvariantCulture));
            line.SetAttribute("stroke", color.Name);
            if (dashed)
                line.SetAttribute("stroke-dasharray", "5,5");
            return line;
        }

        private XmlElement GetCircleSVG(XmlDocument doc, Color color, float x, float y, float r)
        {
            XmlElement circle = doc.CreateElement("circle");
            circle.SetAttribute("cx", x.ToString(CultureInfo.InvariantCulture));
            circle.SetAttribute("cy", y.ToString(CultureInfo.InvariantCulture));
            circle.SetAttribute("r", r.ToString(CultureInfo.InvariantCulture));
            circle.SetAttribute("fill", color.Name);
            return circle;
        }

        private void SetSVGMatrix(XmlElement svg)
        {
            float d = -canvas_rect.Height / canvas_rect.Width;
            float f = canvas_rect.Y * (1 - d) + canvas_rect.Height;
            svg.SetAttribute("transform", "matrix(1 0 0 " +
                d.ToString(CultureInfo.InvariantCulture) + 
                " 0 " +
                f.ToString(CultureInfo.InvariantCulture) + ")"
            );
        }
    }
}