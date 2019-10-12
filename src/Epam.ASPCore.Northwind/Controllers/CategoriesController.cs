using System.Linq;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly INorthwindRepository<Categories> _repository;

        public CategoriesController(INorthwindRepository<Categories> repository)
        {
            _repository = repository;
        }
        
        public IActionResult Index()
        {
            var items = _repository.Get().Select(i => new CategoriesModel
            {
                CategoryId = i.CategoryId,
                Description = i.Description,
                CategoryName = i.CategoryName
            }).ToList();

            return View(items);
        }
    }
}