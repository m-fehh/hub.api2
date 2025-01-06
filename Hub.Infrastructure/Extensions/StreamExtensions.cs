namespace Hub.Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ConvertToByteArray(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
