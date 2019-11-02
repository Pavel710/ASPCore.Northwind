using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly INorthwindRepository<Categories> _categoriesRepository;
        private readonly IImagesService _imagesService;

        public CategoryService(INorthwindRepository<Categories> categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
            _imagesService = new ImagesService();
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

        public List<CategoriesModel> GetCategories()
        {
            try
            {
                return _categoriesRepository.Get().Select(i => new CategoriesModel
                {
                    CategoryId = i.CategoryId,
                    Description = i.Description,
                    CategoryName = i.CategoryName
                }).ToList();
            }
            catch (Exception e)
            {
                Log.Error("Category service error!" + Environment.NewLine + $"{e}");
                throw;
            }
        }

        public CategoryImageModel GetCategoryImage(int categoryId)
        {
            try
            {
                var image = _categoriesRepository.GetByID(categoryId).Picture;
                return new CategoryImageModel
                {
                    ImageStream = new MemoryStream(image),
                    ImageFormat = _imagesService.GetImageFormat(image) ?? ImagesService.DefaultFormat
                };
            }
            catch (Exception e)
            {
                Log.Error("Category service error!" + Environment.NewLine + $"{e}");
                throw;
            }
        }

        public Task<bool> UploadCategoryPicture(IFormFile image, int categoryId)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return Task.FromResult(false);

                byte[] bytesArray;
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    bytesArray = ms.ToArray();
                }

                var category = _categoriesRepository.GetByID(categoryId);
                category.Picture = bytesArray;
                _categoriesRepository.Update(category);

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Log.Error("Category service error!" + Environment.NewLine + $"{e}");
                throw;
            }
        }

        public CategoriesModel GetCategory(int id)
        {
            try
            {
                var category = _categoriesRepository.GetByID(id);
                return new CategoriesModel
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Picture = category.Picture,
                    Description = category.Description
                };
            }
            catch (Exception e)
            {
                Log.Error("Category service error!" + Environment.NewLine + $"{e}");
                throw;
            }
        }
    }
}
