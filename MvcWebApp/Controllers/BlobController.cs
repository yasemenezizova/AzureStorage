using AzureStorageLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcWebApp.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlogStorage _blogStorage;

        public BlobController(IBlogStorage blogStorage)
        {
            _blogStorage = blogStorage;
        }
        public IActionResult Index()
        {
            var names = _blogStorage.GetNames(EContainerName.pictures);
            string blobUrl = $"{_blogStorage.BlobUrl}/{EContainerName.pictures.ToString()}";
            ViewBag.Blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();
            return View();
        }
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            await _blogStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blogStorage.DownloadAsync(fileName,EContainerName.pictures);
            return File(stream,"application/octet-stream", fileName);

        }
    }
}
