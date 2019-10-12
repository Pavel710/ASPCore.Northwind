using System;
using System.Collections.Generic;
using System.Linq;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly INorthwindRepository<Categories> _categoriesRepository;

        public CategoryService(INorthwindRepository<Categories> categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
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
                Log.Error("Category service error!" + Environment.NewLine + $"{e}");
                throw;
            }
        }
    }
}
