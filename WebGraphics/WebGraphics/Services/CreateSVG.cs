using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Xml;
using WebGraphics.Models;

namespace WebGraphics.Services
{
    public class CreateSVG : CreatePicture<XmlElement>
    {
        private XmlDocument doc;
        private XmlElement svg;

        public CreateSVG(GraphicsData expdata) : base(expdata)
        {
        }

        public override XmlElement MakePicture()
        {
            doc = new XmlDocument();
            svg = doc.CreateElement("svg");
            svg.SetAttribute("width", _expdata.Width.ToString());
            svg.SetAttribute("height", _expdata.Height.ToString());
            svg.SetAttribute("viewBox",
                _xmin.ToString(CultureInfo.InvariantCulture) + " " +
                _ymin.ToString(CultureInfo.InvariantCulture) + " " +
                (_xmax - _xmin).ToString(CultureInfo.InvariantCulture) + " " +
                (_ymax - _ymin).ToString(CultureInfo.InvariantCulture)
            );
            //svg.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            SetSVGMatrix();
            doc.AppendChild(svg);

            DrawAxes();
            _expdata.Lines.ForEach(l => DrawLine(l, Color.Black));
            if (string.IsNullOrEmpty(_expdata.Error))
            {
                svg.AppendChild(GetCircleSVG(Color.Blue, _expdata.InterPoint.X, _expdata.InterPoint.Y, 3));
            }
            else if (_expdata.Error.EndsWith("coincident"))
            {
                (PointF first, PointF second) = GetEdges(_expdata.Lines[0]);
                svg.AppendChild(GetLineSVG(Color.Blue, first, second, true));
            }
            return doc.DocumentElement;
        }

        protected override void DrawLine(Line l, Color color)
        {
            (PointF first, PointF second) = GetEdges(l);
            svg.AppendChild(GetLineSVG(color, first, second));
            svg.AppendChild(GetCircleSVG(Color.Green, l.A.X, l.A.Y, 3));
            svg.AppendChild(GetCircleSVG(Color.Red, l.B.X, l.B.Y, 3));
        }

        protected override void DrawAxes()
        {
            svg.AppendChild(GetLineSVG(Color.Black, 0, _ymin, 0, _ymax));
            svg.AppendChild(GetLineSVG(Color.Black, _xmin, 0, _xmax, 0));
            float hx = (_xmax - _xmin) / 10f;
            for (float xs = _xmin + hx; xs <= _xmax; xs += hx)
                svg.AppendChild(GetLineSVG(Color.Black, xs, -1f, xs, 1f));

            float hy = (_ymax - _ymin) / 10f;
            for (float ys = _ymin + hy; ys <= _ymax; ys += hy)
                svg.AppendChild(GetLineSVG(Color.Black, -1f, ys, 1f, ys));
        }


        private XmlElement GetLineSVG(Color color, PointF a, PointF b, bool dashed = false)
        {
            return GetLineSVG(color, a.X, a.Y, b.X, b.Y, dashed);
        }

        private XmlElement GetLineSVG(Color color, float x1, float y1, float x2, float y2, bool dashed = false)
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

        private XmlElement GetCircleSVG(Color color, float x, float y, float r)
        {
            XmlElement circle = doc.CreateElement("circle");
            circle.SetAttribute("cx", x.ToString(CultureInfo.InvariantCulture));
            circle.SetAttribute("cy", y.ToString(CultureInfo.InvariantCulture));
            circle.SetAttribute("r", r.ToString(CultureInfo.InvariantCulture));
            circle.SetAttribute("fill", color.Name);
            return circle;
        }

        private void SetSVGMatrix()
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