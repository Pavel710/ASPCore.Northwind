using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.ConsoleClient.Models;

namespace Epam.ASPCore.Northwind.ConsoleClient
{
    class Program
    {
        private const string BaseAddress = "http://localhost:50142/";
        private const string CategoriesAddress = "api/NorthwindAPI/categories";
        private const string ProductsAddress = "api/NorthwindAPI/products";

        private static readonly HttpClient Client = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            Client.BaseAddress = new Uri(BaseAddress);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var categoriesUrl = BaseAddress + CategoriesAddress;
                var productsUrl = BaseAddress + ProductsAddress;

                var categories = await GetCollectionAsync<CategoriesModel>(categoriesUrl);
                ShowCategories(categories);

                var products = await GetCollectionAsync<ProductsModel>(productsUrl);
                ShowProducts(products);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static async Task<List<T>> GetCollectionAsync<T>(string path)
        {
            List<T> collection = null;
            HttpResponseMessage response = await Client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                collection = await response.Content.ReadAsAsync<List<T>>();
            }
            return collection;
        }

        static void ShowCategories(List<CategoriesModel> categories)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("#CATEGORIES START#\n");
            Console.ResetColor();
            foreach (var category in categories)
            {
                Console.WriteLine($"Category ID: {category.CategoryId}\n" +
                                  $"Name: {category.CategoryName}\n" +
                                  $"Description: {category.Description}\n");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("#CATEGORIES END#\n");
            Console.ResetColor();
        }

        static void ShowProducts(List<ProductsModel> products)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("#PRODUCTS START#\n");
            Console.ResetColor();
            foreach (var product in products)
            {
                Console.WriteLine($"Product ID: {product.ProductId}\n" +
                                  $"Name: {product.ProductName}\n" +
                                  $"Units in stock: {product.UnitsInStock}\n" +
                                  $"Units on order: {product.UnitsOnOrder}\n" +
                                  $"Unit price: {product.UnitPrice}\n" +
                                  $"Quantity per unit: {product.QuantityPerUnit}\n" +
                                  $"Reorder level: {product.ReorderLevel}\n" +
                                  $"Discontinued: {product.Discontinued}\n" +
                                  $"Category: {product.Category.CategoryName}\n");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("#PRODUCTS END#\n");
            Console.ResetColor();
        }
    }
}
