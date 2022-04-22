using AzureStorageLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MvcWebApp.Controllers
{
    public class PictureController : Controller
    {
        private readonly INoSqlStorage<UserPicture> _noSqlStorage;
        private readonly IBlogStorage _blobStorage;

        public PictureController(INoSqlStorage<UserPicture> noSqlStorage, IBlogStorage blobStorage)
        {
            _noSqlStorage = noSqlStorage;
            _blobStorage = blobStorage;
        }
        public string UserId { get; set; } = "12345";
        public string City { get; set; } = "Baku";

        public async Task<IActionResult> Index()
        {
            List<FileBlob> fileBlobs = new List<FileBlob>();

            var user = await _noSqlStorage.Get(UserId, City);

            if (user != null)
            {
                user.Paths.ForEach(x =>
                {
                    fileBlobs.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}/{x}" });

                });
            }
            ViewBag.fileBlobs = fileBlobs;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
        {
            List<string> pictureList = new List<string>();
            foreach (var picture in pictures)
            {
                var newPictureName = $"{Guid.NewGuid()}{Path.GetExtension(picture.FileName)}";
                await _blobStorage.UploadAsync(picture.OpenReadStream(), newPictureName, EContainerName.pictures);
                pictureList.Add(newPictureName);
            }

            var isUser = await _noSqlStorage.Get(UserId, City);
            if(isUser != null)
            {
                pictureList.AddRange(isUser.Paths);
                isUser.Paths = pictureList;
                //isUsere neye gore add olun,ur onu arasdir
            }
            else
            {
                isUser=new UserPicture();
                isUser.RowKey = UserId;
                isUser.PartitionKey = City;
                isUser.Paths= pictureList;
            }
                await _noSqlStorage.Add(isUser);

            return RedirectToAction("Index");
        }
    }
}
