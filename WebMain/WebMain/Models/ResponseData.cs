using System.Xml;

namespace WebMain.Models
{
    public class ResponseData
    {
        public float A { get; set; }
        public float B { get; set; }

        public string Image { get; set; }

        public string Error {  get; set; }

        public XmlElement SvgImage { get; set; }
    }
}