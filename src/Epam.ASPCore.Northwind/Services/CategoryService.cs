using System;
using System.Collections.Generic;
using System.Linq;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly INorthwindRepository<Categories> _categoriesRepository;
        private readonly ILogger _logger;

        public CategoryService(INorthwindRepository<Categories> categoriesRepository,
            ILogger logger)
        {
            _categoriesRepository = categoriesRepository;
            _logger = logger;
        }

        public List<SelectListItem> GetCategoriesSelectedList(int? selectedItemId = null)
        {
            try
            {
                List<SelectListItem> list = new List<SelectListItem>();
                var categories = _categoriesRepository.Get().ToList();

                foreach (var category in categories)
                {
                    var selected = category.CategoryId == selectedItemId;
                    list.Add(new SelectListItem
                    {
                        Value = category.CategoryId.ToString(),
                        Text = category.CategoryName,
                        Selected = selected
                    });
                }

                return list;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Category service error!");
                throw;
            }
        }
    }
}
