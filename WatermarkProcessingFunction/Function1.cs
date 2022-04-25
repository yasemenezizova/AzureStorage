using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MvcWebApp.Models;

namespace WatermarkProcessingFunction
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task RunAsync([QueueTrigger("watermarkqueue")] PictureWatermarkQueue pictureWatermarkQueue, ILogger log)
        {
            ConnectionStrings.AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=projectbyasaman;AccountKey=/i3ZOXxgBMGCCE8uX7p1FH3H3Qmd+RgZuIeiWk1M3bRRkJ4E4JRzCR/YpWqzJ0xPI/RZIHcEiI3v+AStEtfg1A==;BlobEndpoint=https://projectbyasaman.blob.core.windows.net/;QueueEndpoint=https://projectbyasaman.queue.core.windows.net/;TableEndpoint=https://projectbyasaman.table.core.windows.net/;FileEndpoint=https://projectbyasaman.file.core.windows.net/;";
            IBlogStorage blobStorage = new BlobStorage();
            INoSqlStorage<UserPicture> noSqlStorage = new TableStorage<UserPicture>();

            foreach (var item in pictureWatermarkQueue.Pictures)
            {
                using var stream = await blobStorage.DownloadAsync(item, EContainerName.pictures);

                using var memoryStream = AddWaterMark(pictureWatermarkQueue.WatermarkText, stream);

                await blobStorage.UploadAsync(memoryStream, item, EContainerName.watermark);

                log.LogInformation($"watermark added to {item}.");
            }

            var userpicture = await noSqlStorage.Get(pictureWatermarkQueue.UserId, pictureWatermarkQueue.City);

            if (userpicture.WatermarkRawPaths != null)
            {
                pictureWatermarkQueue.Pictures.AddRange(userpicture.WatermarkPaths);
            }

            userpicture.WatermarkPaths = pictureWatermarkQueue.Pictures;

            await noSqlStorage.Add(userpicture);

            //HttpClient httpClient = new HttpClient();

            //var response = await httpClient.GetAsync("https://localhost:44332/api/Notifications/CompleteWatermarkProcess/" + pictureWatermarkQueue.ConnectionId);

            log.LogInformation($"Client({pictureWatermarkQueue.ConnectionId}) connected");
        }

        public MemoryStream AddWaterMark(string watermarkText, Stream PictureStream)
        {
            MemoryStream memory = new MemoryStream();
            using (Image image = Bitmap.FromStream(PictureStream))
            {
                using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(tempBitmap))
                    {
                        graphics.DrawImage(image, 0, 0);
                        var font = new Font(FontFamily.GenericSerif, 25, FontStyle.Bold);
                        var color = Color.FromArgb(255, 0, 0);
                        var brush = new SolidBrush(color);
                        var point = new Point(20, image.Height - 50);
                        graphics.DrawString(watermarkText, font, brush, point);
                        tempBitmap.Save(memory, ImageFormat.Png);

                    }
                }

            }
            memory.Position = 0;
            return memory;
        }
    }
}
