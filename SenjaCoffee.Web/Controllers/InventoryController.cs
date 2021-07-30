using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SenjaCoffee.Data.Models;
using SenjaCoffee.Services.Inventory;
using SenjaCoffee.Web.Serialization;
using SenjaCoffee.Web.ViewModels;

namespace SenjaCoffee.Web.Controllers
{
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet("/api/inventory")]
        public ActionResult GetCurrentInventory()
        {
            _logger.LogInformation("Getting all inventory....");
            var inventory = _inventoryService.GetCurrentInventory()
                .Select(productInventory => new ProductInventoryModel
                {
                    Id = productInventory.Id,
                    Product = ProductMapper.SerializeProductModel(productInventory.Product),
                    IdealQuantity = productInventory.IdealQuantity,
                    QuantityOnHand = productInventory.QuantityOnHand
                })
                .OrderBy(inv => inv.Product.Name)
                .ToList();

            return Ok(inventory);
        }

        [HttpPatch("/api/inventory")]
        public ActionResult UpdateInventory([FromBody] ShipmentModel shipment)
        {
            _logger.LogInformation($"Updating inventory for {shipment.ProductId} - Adjustment: {shipment.Adjustment}");
            var id = shipment.ProductId;
            var adjustment = shipment.Adjustment;
            var inventory = _inventoryService.UpdateUnitsAvailable(id, adjustment);
            return Ok(inventory);
        }
    }
}