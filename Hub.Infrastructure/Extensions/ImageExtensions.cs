using System.Drawing;

namespace Hub.Infrastructure.Extensions
{
    public static class ImageExtensions
    {
        public static byte[] ImageToByteArray(this Image imageIn)
        {
            if (imageIn == null) return null;

            var ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
