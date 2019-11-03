using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Epam.ASPCore.Northwind.Tests.API
{
    public class UnitTest1
    {
        private readonly HttpClient _client;
        private readonly NorthwindAPIClient _northwindClient;

        public UnitTest1()
        {
            _client = new HttpClient();
            _northwindClient = new NorthwindAPIClient(_client);
        }

        [Fact]
        public async void Test1()
        {
            var categories = await _northwindClient.GetCategoriesCollectionAsync();

            Assert.IsAssignableFrom<ICollection<CategoriesModel>>(categories);
            Assert.Equal(8, categories.Count);
        }

        [Fact]
        public async void Test2()
        {
            try
            {
                var categoryImage = await _northwindClient.GetCategoryImageAsync(10);
            }
            catch (Exception e)
            {
                Assert.IsAssignableFrom<ApiException>(e);
            }
        }
    }
}
