using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SenjaCoffee.Services.Product;
using SenjaCoffee.Web.Serialization;

namespace SenjaCoffee.Web.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        
        [HttpGet("/api/product")]
        public ActionResult GetProduct() 
        {
           _logger.LogInformation("Getting all products");
           var products = _productService.GetAllProducts();
           
           var productViewModels = products
                .Select(product => ProductMapper.SerializeProductModel(product));
           
           return Ok(productViewModels);
        }
    }
}