using System.Drawing;
using System.Drawing.Drawing2D;
using WebGraphics.Models;

namespace WebGraphics.Services
{
    public class CreateBitMap : CreatePicture<Bitmap>
    {
        private Graphics g;
        private Bitmap bm;

        public CreateBitMap(GraphicsData expdata) : base(expdata)
        {
        }

        public override Bitmap MakePicture()
        {
            bm = new Bitmap(_expdata.Width, _expdata.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Transform = new Matrix(canvas_rect, canvas_pts);

            DrawAxes();
            _expdata.Lines.ForEach(l => DrawLine(l, Color.Black));
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

        protected override void DrawLine(Line l, Color color)
        {
            (PointF first, PointF second) = GetEdges(l);
            g.DrawLine(new Pen(color, 0), first, second);
            g.FillEllipse(new SolidBrush(Color.Green), l.A.X - _factor / 2, l.A.Y - _factor / 2, _factor, _factor);
            g.FillEllipse(new SolidBrush(Color.Red), l.B.X - _factor / 2, l.B.Y - _factor / 2, _factor, _factor);
        }

        protected override void DrawAxes()
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