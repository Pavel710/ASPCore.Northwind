using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Epam.ASPCore.Northwind.Tests.API.Models;
using Newtonsoft.Json;
using Xunit;

namespace Epam.ASPCore.Northwind.Tests.API
{
    public class APITests
    {
        private readonly HttpClient _client;
        private readonly NorthwindAPIClient _northwindClient;
        private CategoriesModel _categoriesModel;

        public APITests()
        {
            _client = new HttpClient();
            _northwindClient = new NorthwindAPIClient(_client);
            _categoriesModel = new CategoriesModel(2, null, null, null, null);
        }

        [Fact]
        public async void GetCategoriesCollection_Test()
        {
            var categories = await _northwindClient.GetCategoriesCollectionAsync();

            Assert.IsAssignableFrom<ICollection<CategoriesModel>>(categories);
            Assert.Equal(8, categories.Count);
        }

        [Fact]
        public async void GetProductsCollection_Test()
        {
            var products = await _northwindClient.GetProductsCollectionAsync();

            Assert.IsAssignableFrom<ICollection<ProductsModel>>(products);
            Assert.Equal(7, products.Count);
        }

        [Fact]
        public async void CreateProduct_Test()
        {
            var productsModel = new ProductsModel(_categoriesModel, true, null, 0, "testProduct1", "7 test boxes", 5, null, (decimal)55.9, 14, 11);

            var createdResponse = await _northwindClient.PostCreateProductAsync(productsModel);

            Assert.True(createdResponse.StatusCode == 200);
            StreamReader reader = new StreamReader(createdResponse.Stream);
            string jsonString = reader.ReadToEnd();
            ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            Assert.True(responseModel.Success);
        }

        [Fact]
        public async void CreateProduct_Failed_ProductName_Required_Test()
        {
            try
            {
                var productsModel = new ProductsModel(_categoriesModel, true, null, 0, string.Empty, "7 test boxes", 5, null, (decimal)55.9, 14, 11);
                var createdResponse = await _northwindClient.PostCreateProductAsync(productsModel);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 400);
                ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(apiEx.Response);
                Assert.Equal("The product name field is required.", responseModel.ProductName.FirstOrDefault());
            }
        }

        [Fact]
        public async void GetNotExistedCategoryImage_Test()
        {
            try
            {
                var categoryImage = await _northwindClient.GetCategoryImageAsync(100);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 500);
            }
        }
    }
}
