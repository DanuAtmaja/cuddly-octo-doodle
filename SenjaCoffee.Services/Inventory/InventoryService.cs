using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SenjaCoffee.Data;
using SenjaCoffee.Data.Models;

namespace SenjaCoffee.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly SenjaDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(SenjaDbContext dbContext, ILogger<InventoryService> logger)
        {
            _db = dbContext;
            _logger = logger;
        }
        
        /// <summary>
        /// Returns all current inventory from the database 
        /// </summary>
        /// <returns></returns>
        public List<ProductInventory> GetCurrentInventory()
        {
            return _db.ProductInventories
                .Include(inventory => inventory.Product)
                .Where(inventory => !inventory.Product.IsArchived)
                .ToList();
        }
        
        /// <summary>
        /// Updates number of units avaiable of the provided product id
        /// Adjusts QuantityOnHand by adjustment value
        /// </summary>
        /// <param name="id">productId</param>
        /// <param name="adjustment">number of units added / removed from inventory</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ServiceResponse<ProductInventory> UpdateUnitsAvailable(int id, int adjustment)
        {
            var now = DateTime.UtcNow;
            
            try
            {
                var inventory = _db.ProductInventories
                    .Include(productInventory => productInventory.Product)
                    .First(productInventory => productInventory.Product.Id == id);

                inventory.QuantityOnHand += adjustment;

                try
                {
                    CreateSnapshot(inventory);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error creating inventory snapshot.");
                    _logger.LogError(e.StackTrace);
                }
                
                _db.SaveChanges();

                return new ServiceResponse<ProductInventory>
                {
                    IsSuccess = true,
                    Data = inventory,
                    Message = $"Product {id} inventory adjusted",
                    Time = now
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<ProductInventory>
                {
                    IsSuccess = false,
                    Data = null,
                    Message = e.StackTrace,
                    Time = now
                }; 
            }
        }

        /// <summary>
        /// Gets a ProductInventory instance by Product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductInventory GetByProductId(int productId)
        {
            return _db.ProductInventories
                .Include(inventory => inventory.Product)
                .FirstOrDefault(inventory => inventory.Product.Id == productId);
        }

        /// <summary>
        /// Create a snapshot record using the provided ProductInventory instance
        /// </summary>
        /// <param name="inventory"></param>
        private void CreateSnapshot(ProductInventory inventory)
        {
            var now = DateTime.UtcNow;

            var snapshot = new ProductInventorySnapshot
            {
                SnapshotTime = now,
                Product = inventory.Product,
                QuantityOnHand = inventory.QuantityOnHand
            };

            _db.Add(snapshot);
            _db.SaveChanges();
        }

        /// <summary>
        /// Return Snapshot history for the previous 6 hours
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<ProductInventorySnapshot> GetSnapshotHistory()
        {
            var earliest = DateTime.UtcNow - TimeSpan.FromHours(6);
            return _db.ProductInventorySnapshots
                .Include(snapshot => snapshot.Product)
                .Where(snapshot => snapshot.SnapshotTime > earliest && !snapshot.Product.IsArchived).ToList();
        }
    }
}