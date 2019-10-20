using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Middleware
{
    public class RequestResponseImagesMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseImagesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    var pathRequest = context.Request.Path.Value;
                    var indexStr = pathRequest.Contains("CategoryImages/") ? pathRequest.IndexOf("CategoryImages/") + "CategoryImages/".Length : -1;
                    if (indexStr != -1)
                    {
                        var fileName = pathRequest.Contains("CategoryImages/") ? pathRequest.Substring(indexStr) : null;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            DirectoryInfo root = new DirectoryInfo("D:\\CacheImages");
                            FileInfo[] listFiles = root.GetFiles($"{fileName}.*");
                            if (listFiles.Length == 0)
                            {
                                await _next(context);

                                memStream.Position = 0;
                                if (context.Response.ContentType == "application/octet-stream")
                                {
                                    byte[] data = memStream.ToArray();
                                    var format = ValidateImageFormat(data);
                                    if (!string.IsNullOrEmpty(format))
                                    {
                                        if (context.Request.Path.HasValue)
                                        {
                                            if (indexStr != -1)
                                            {
                                                if (!string.IsNullOrEmpty(fileName))
                                                {
                                                    ByteArrayToFile(fileName, data, format);
                                                }
                                            }
                                        }
                                    }
                                }

                                memStream.Position = 0;
                                await memStream.CopyToAsync(originalBody);
                            }
                            else
                            {
                                var cacheMemoryStream = new MemoryStream(File.ReadAllBytes(listFiles[0].FullName));
                                context.Response.Headers.Add("cacheImage", "true");
                                cacheMemoryStream.Position = 0;
                                await cacheMemoryStream.CopyToAsync(originalBody);
                            }
                        }
                    }
                    else
                    {
                        await _next(context);
                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    }
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        private string ValidateImageFormat(byte[] bytes)
        { 
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };    // PNG
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return "bmp";

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return "gif";

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return "png";

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return "jpeg";

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return "jpeg2";

            return string.Empty;
        }

        private void ByteArrayToFile(string fileName, byte[] byteArray, string format)
        {
            try
            {
                string directory = "D:\\CacheImages";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var filePath = directory + "\\" + fileName + "." + format;
                if (!File.Exists(filePath))
                {
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(byteArray, 0, byteArray.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Exception caught in process: {ex}");
            }
        }
    }
}
