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
        public async Task<IActionResult> Index()
        {
            var names = _blogStorage.GetNames(EContainerName.pictures);
            string blobUrl = $"{_blogStorage.BlobUrl}/{EContainerName.pictures.ToString()}";
            ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();

            ViewBag.logs = await _blogStorage.GetLogAsync("controller.txt");
            return View();
        }
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            await _blogStorage.SetLogAsync("Upload begin to work", "controller.txt");

            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            await _blogStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);
            await _blogStorage.SetLogAsync("Upload finished","controller.txt");
            return RedirectToAction("Index", ViewBag.logs);
        }
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blogStorage.DownloadAsync(fileName, EContainerName.pictures);
            return File(stream, "application/octet-stream", fileName);

        }

        public async Task<IActionResult> Delete(string fileName)
        {
            await _blogStorage.DeleteAsync(fileName, EContainerName.pictures);
            return RedirectToAction("Index");
        }
    }
}
