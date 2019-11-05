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

            var response = await _northwindClient.PostCreateProductAsync(productsModel);

            Assert.True(response.StatusCode == 200);
            StreamReader reader = new StreamReader(response.Stream);
            string jsonString = reader.ReadToEnd();
            ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            Assert.True(responseModel.Success);
        }

        [Fact]
        public async void CreateProduct_FailedRequestData_Test()
        {
            try
            {
                var productsModel = new ProductsModel(_categoriesModel, true, null, 0, string.Empty, string.Empty, 5, null, -1000, 1000, 1000);
                var response = await _northwindClient.PostCreateProductAsync(productsModel);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 400);
                ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(apiEx.Response);
                Assert.Equal("The product name field is required.", responseModel.ProductName.FirstOrDefault());
                Assert.Equal("The quantity per unit field is required.", responseModel.QuantityPerUnit.FirstOrDefault());
                Assert.Equal("The field UnitPrice must be between 0 and 9999.99.", responseModel.UnitPrice.FirstOrDefault());
                Assert.Equal("The field UnitsInStock must be between 0 and 200.", responseModel.UnitsInStock.FirstOrDefault());
                Assert.Equal("The field UnitsOnOrder must be between 0 and 200.", responseModel.UnitsOnOrder.FirstOrDefault());
            }
        }

        [Fact]
        public async void UpdateProduct_Test()
        {
            var productsList = await _northwindClient.GetProductsCollectionAsync();
            var productFromList = productsList.FirstOrDefault();
            var productModel = new ProductsModel(productFromList.Category, false, productFromList.OrderDetails, productFromList.ProductId, productFromList.ProductName + " test update", productFromList.QuantityPerUnit, 32, productFromList.Supplier, 120, 40, 50);

            var response = await _northwindClient.PutUpdateProductAsync(productModel);

            Assert.True(response.StatusCode == 200);
            StreamReader reader = new StreamReader(response.Stream);
            string jsonString = reader.ReadToEnd();
            ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            Assert.True(responseModel.Success);

            var updatedProductsList = await _northwindClient.GetProductsCollectionAsync();
            var updatedProduct = updatedProductsList.FirstOrDefault();
            Assert.Equal(productModel.ProductId, updatedProduct.ProductId);
            Assert.Equal(productModel.ProductName, updatedProduct.ProductName);
            Assert.Equal(productModel.Discontinued, updatedProduct.Discontinued);
            Assert.Equal(productModel.ReorderLevel, updatedProduct.ReorderLevel);
            Assert.Equal(productModel.UnitPrice, updatedProduct.UnitPrice);
            Assert.Equal(productModel.UnitsInStock, updatedProduct.UnitsInStock);
            Assert.Equal(productModel.UnitsOnOrder, updatedProduct.UnitsOnOrder);
        }

        [Fact]
        public async void UpdateProduct_FailedRequestData_Test()
        {
            try
            {
                var productsList = await _northwindClient.GetProductsCollectionAsync();
                var productFromList = productsList.FirstOrDefault();
                var productsModel = new ProductsModel(productFromList.Category, productFromList.Discontinued, productFromList.OrderDetails, productFromList.ProductId, string.Empty, string.Empty, productFromList.ReorderLevel, productFromList.Supplier, -1000, 1000, 1000);
                var response = await _northwindClient.PutUpdateProductAsync(productsModel);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 400);
                ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(apiEx.Response);
                Assert.Equal("The product name field is required.", responseModel.ProductName.FirstOrDefault());
                Assert.Equal("The quantity per unit field is required.", responseModel.QuantityPerUnit.FirstOrDefault());
                Assert.Equal("The field UnitPrice must be between 0 and 9999.99.", responseModel.UnitPrice.FirstOrDefault());
                Assert.Equal("The field UnitsInStock must be between 0 and 200.", responseModel.UnitsInStock.FirstOrDefault());
                Assert.Equal("The field UnitsOnOrder must be between 0 and 200.", responseModel.UnitsOnOrder.FirstOrDefault());
            }
        }

        [Fact]
        public async void DeleteProduct_Test()
        {
            var productsList = await _northwindClient.GetProductsCollectionAsync();
            var deletedFirstItemInListId = productsList.FirstOrDefault().ProductId;

            var response = await _northwindClient.DeleteProductAsync(deletedFirstItemInListId);

            Assert.True(response.StatusCode == 200);
            StreamReader reader = new StreamReader(response.Stream);
            string jsonString = reader.ReadToEnd();
            ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            Assert.True(responseModel.Success);
        }

        [Fact]
        public async void DeleteProduct_Failed_Test()
        {
            try
            {
                var response = await _northwindClient.DeleteProductAsync(101123423);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 500);
            }
        }

        [Fact]
        public async void GetCategoryImage_Test()
        {
            var categoryImage = await _northwindClient.GetCategoryImageAsync(3);

            Assert.Equal("image/*", categoryImage.Headers["Content-Type"].FirstOrDefault());
            Assert.False(string.IsNullOrEmpty(categoryImage.Headers["Content-Length"].FirstOrDefault()));
            Assert.True(Convert.ToInt32(categoryImage.Headers["Content-Length"].FirstOrDefault()) > 0);
        }

        [Fact]
        public async void GetNotExistedCategoryImage_Test()
        {
            try
            {
                var response = await _northwindClient.GetCategoryImageAsync(100);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 500);
            }
        }

        [Fact]
        public async void UpdateCategoryImage_Test()
        {
            var testDirectory = Directory.GetFiles("../../../TestResources").Where(x => x.Contains("Test_image_category.jpeg")).FirstOrDefault();
            var testImage = File.ReadAllBytes(testDirectory);
            MemoryStream ms = new MemoryStream(testImage);
            FileParameter fileParameter = new FileParameter(ms);
            var response = await _northwindClient.UpdateCategoryImageAsync(fileParameter, 5);

            Assert.True(response.StatusCode == 200);
            StreamReader reader = new StreamReader(response.Stream);
            string jsonString = reader.ReadToEnd();
            ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            Assert.True(responseModel.Success);

            var categoryImage = await _northwindClient.GetCategoryImageAsync(5);
            Assert.True(Convert.ToInt32(categoryImage.Headers["Content-Length"].FirstOrDefault()) == testImage.Length);
        }

        [Fact]
        public async void UpdateNotExistedCategoryImage_Test()
        {
            try
            {
                var testDirectory = Directory.GetFiles("../../../TestResources").Where(x => x.Contains("Test_image_category.jpeg")).FirstOrDefault();
                var testImage = File.ReadAllBytes(testDirectory);
                MemoryStream ms = new MemoryStream(testImage);
                FileParameter fileParameter = new FileParameter(ms);
                var response = await _northwindClient.UpdateCategoryImageAsync(fileParameter, 100);
            }
            catch (Exception e)
            {
                var apiEx = Assert.IsAssignableFrom<ApiException>(e);
                Assert.True(apiEx.StatusCode == 500);
            }
        }
    }
}
