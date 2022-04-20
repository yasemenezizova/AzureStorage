using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;
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
            ViewBag.products = _noSqlStorage.All().ToList();
            return View();
        }
        public async Task <IActionResult> Create(Product product)
        {
            product.RowKey=Guid.NewGuid().ToString();
            product.PartitionKey = "Pens";
            await _noSqlStorage.Add(product);
            return RedirectToAction("Index");
        }
    }
}
