using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PosWarehouse.DAL;
using PosWarehouse.DAL.Ecom;
using PosWarehouse.ViewModel.Ecom;

namespace PosWarehouse.Controllers.Api.Ecom
{
    public class ProductController : ApiController
    {
        private readonly EcomDal _ecomDal;

        public ProductController()
        {
            _ecomDal = new EcomDal();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetProducts()
        {
            var products = await _ecomDal.GetAllProductInfoForEcom();

            if (products.Any())
            {
                return Ok(products);
            }
            return BadRequest();
        }

        //[HttpGet]
        //public async Task<IHttpActionResult> UpdateProduct(int productId)
        //{
        //    var result = await _ecomDal.UpdateEcomProduct(productId);

        //    if (result)
        //        return Ok("Update Successfully");

        //    return BadRequest();
        //}

        //[HttpGet]
        //public async Task<IHttpActionResult> LoadData()
        //{
        //    var result = await _ecomDal.LoadData();

        //    if (result == "TRUE")
        //        return Ok();

        //    return BadRequest();
        //}

        //[HttpGet]
        //public async Task<IHttpActionResult> GetProductCategory()
        //{
        //    var productCategory = await _ecomDal.GetProductCategory();

        //    if (productCategory.Any())
        //    {
        //        foreach (var category in productCategory)
        //        {
        //            category.ProductSubCategories = await _ecomDal.GetProductSubCategory(category.Id);
        //        }
        //    }
        //    return Ok(productCategory);
        //}
    }
}
