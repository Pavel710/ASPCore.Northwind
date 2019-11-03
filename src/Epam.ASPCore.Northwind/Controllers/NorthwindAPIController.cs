using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Epam.ASPCore.Northwind.WebUI.Models;
using Epam.ASPCore.Northwind.WebUI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epam.ASPCore.Northwind.WebUI.Controllers
{
    [Route("api/NorthwindAPI")]
    [ApiController]
    public class NorthwindAPIController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public NorthwindAPIController(
            IProductService productService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        #region Categories

        [HttpGet("categories")]
        public ActionResult<List<CategoriesModel>> GetCategoriesCollection()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                return _categoryService.GetCategories();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpGet("categoryImage")]
        public ActionResult GetCategoryImage(int id)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                Stream categoryImageStream = _categoryService.GetCategoryImageStream(id);

                if (categoryImageStream.Length == 0)
                    return NotFound();

                return File(categoryImageStream, "image/*");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut("categoryImageUpdate")]
        public async Task<ActionResult> UpdateCategoryImage(IFormFile uploadedImage, int categoryId)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            
            try
            {
                return new JsonResult(new { Success = await _categoryService.UploadCategoryPicture(uploadedImage, categoryId) });
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        #endregion

        #region Products

        [HttpGet("products")]
        public ActionResult<List<ProductsModel>> GetProductsCollection()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            
            try
            {
                return _productService.GetProducts();
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("productCreate")]
        public ActionResult PostCreateProduct(ProductsModel model)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            try
            {
                _productService.SaveProduct(model);
                return new JsonResult(new { Success = true });
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPut("productUpdate")]
        public ActionResult PutUpdateProduct(ProductsModel model)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            try
            {
                _productService.UpdateProduct(model);
                return new JsonResult(new { Success = true });
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpDelete("productDelete")]
        public ActionResult DeleteProduct(int productId)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            try
            {
                _productService.DeleteProduct(productId);
                return new JsonResult(new { Success = true });
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        #endregion
    }
}