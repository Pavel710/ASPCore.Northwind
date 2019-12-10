using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.WebUI.Middleware.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Middleware
{
    [AllowAnonymous]
    public class RequestResponseImagesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ImageOptions _options;
        private Timer _timer;
        
        public RequestResponseImagesMiddleware(
            RequestDelegate next,
            ImageOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string queryStringForImages = "CategoryImages/";

            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    var pathRequest = context.Request.Path.Value;
                    var subStringExists = pathRequest.Contains(queryStringForImages) ? pathRequest.IndexOf(queryStringForImages) + queryStringForImages.Length : -1;
                    var fileName = pathRequest.Contains(queryStringForImages) ? pathRequest.Substring(subStringExists) : null;
                    if (subStringExists != -1 && !string.IsNullOrEmpty(fileName))
                    {
                        if (!Directory.Exists(_options.Path))
                        {
                          Directory.CreateDirectory(_options.Path);
                        }

                        DirectoryInfo root = new DirectoryInfo(_options.Path);
                        FileInfo[] listFiles = root.GetFiles($"{fileName}.*");
                        await StartExpirationAsync();
                        if (listFiles.Length == 0 && listFiles.Length <= _options.MaxCountItem)
                        {
                            await _next(context);

                            memStream.Position = 0;
                            if (context.Response.ContentType == "image/*")
                            {
                                byte[] data = memStream.ToArray();
                                var format = GetImageFormat(data);
                                if (!string.IsNullOrEmpty(format))
                                {
                                    SaveToCache(fileName, data, format);
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

        private string GetImageFormat(byte[] bytes)
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

        private void SaveToCache(string fileName, byte[] byteArray, string format)
        {
            try
            {
              var filePath = _options.Path + "\\" + fileName + "." + format;
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

        private Task StartExpirationAsync()
        {
            if(_timer == null)
                _timer = new Timer(ClearCache, null, TimeSpan.FromMinutes(_options.ExpirationMinutes),
                    TimeSpan.FromMinutes(_options.ExpirationMinutes));

            return Task.CompletedTask;
        }

        private void ClearCache(object state)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_options.Path);

            if (directoryInfo.GetFiles().Length != 0)
            {
                Array.ForEach(Directory.GetFiles(_options.Path), File.Delete);
            }
        }
    }
}
