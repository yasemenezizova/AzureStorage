using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcWebApp.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly INoSqlStorage<Product> _noSqlStorage;

        public TableStorageController(INoSqlStorage<Product> noSqlStorage)
        {
            this._noSqlStorage = noSqlStorage;
        }

        public IActionResult Index()
        {
            ViewBag.Process = ProcessEnum.Get;
            ViewBag.products = _noSqlStorage.All().ToList();
            return View();
        }

        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Pens";
            await _noSqlStorage.Add(product);
            return RedirectToAction("Index");
        }

        public IActionResult Update(string rowKey, string partitionKey)
        {
            var product = _noSqlStorage.Query(x => x.RowKey == rowKey);
            ViewBag.Process = ProcessEnum.Update;
            ViewBag.products = _noSqlStorage.All().ToList();
            return View("Index", product.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            product.ETag = "*";
            ViewBag.Process = ProcessEnum.Update;
            ViewBag.products = _noSqlStorage.All().ToList();
            await _noSqlStorage.Update(product);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string rowKey, string partitionKey)
        {
            await _noSqlStorage.Delete(rowKey, partitionKey);
            return RedirectToAction("Index");
        }
        public IActionResult Query(double price)
        {
            ViewBag.products = _noSqlStorage.Query(x => x.Price > price).ToList();
            ViewBag.Process = ProcessEnum.Get;
            return View("Index");
        }

    }
}
