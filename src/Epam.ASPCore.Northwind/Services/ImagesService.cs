using System.Linq;
using System.Text;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class ImagesService : IImagesService
    {
        public const string DefaultFormat = "jpeg";

        public string GetImageFormat(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");
            var gif = Encoding.ASCII.GetBytes("GIF");
            var png = new byte[] { 137, 80, 78, 71 };
            var jpeg = new byte[] { 255, 216, 255, 224 };

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return "bmp";

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return "gif";

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return "png";

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return "jpeg";

            return string.Empty;
        }
    }
}
