using System.Collections.Generic;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Controllers;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Epam.ASPCore.Northwind.Tests
{
    public class CategoryServiceUnitTests
    {
        [Fact]
        public void GetCategoriesList_Test()
        {
            // Arrange
            var mockRepo = new Mock<INorthwindRepository<Categories>>();
            List<Categories> listCategories = new List<Categories> {new Categories {CategoryId = 1}};
            mockRepo.Setup(repo => repo.Get())
                .Returns(() => listCategories);
            var service = new CategoryService(mockRepo.Object);

            // Act
            var result = service.GetCategories();

            // Assert
            Assert.IsType<List<CategoriesModel>>(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].CategoryId);
        }

        [Fact]
        public void GetCategoriesList_Test2()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryService>();
            List<CategoriesModel> listCategories = new List<CategoriesModel> { new CategoriesModel { CategoryId = 1 } };
            mockRepo.Setup(repo => repo.GetCategories())
                .Returns(() => listCategories);
            var controller = new CategoriesController(mockRepo.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CategoriesModel>>(
                viewResult.ViewData.Model);
            Assert.Single(model);
            Assert.Equal(1, model[0].CategoryId);
        }
    }
}
