using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;

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
            ViewBag.products = _noSqlStorage.All();
            return View();
        }
    }
}
