using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace WebGraphics.Models
{
    public class CreateImage
    {
        private byte[] _data;
        private string _encoded;

        public string ImageString { get => _encoded; }

        public CreateImage(Bitmap bm)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bm.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                _data = new byte[stream.Length];
                stream.Read(_data, 0, (int)stream.Length);
                stream.Close();
            }
            _encoded = Convert.ToBase64String(_data);
        }
    }
}