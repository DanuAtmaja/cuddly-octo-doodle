using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SenjaCoffee.Data.Models;
using SenjaCoffee.Services.Customer;
using SenjaCoffee.Web.Serialization;
using SenjaCoffee.Web.ViewModels;

namespace SenjaCoffee.Web.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpPost("/api/customer")]
        public ActionResult CreateCustomer([FromBody] CustomerModel customer) 
        {
           _logger.LogInformation("Creating a new customer"); 
           customer.CreatedOn = DateTime.UtcNow;
           customer.UpdatedOn = DateTime.UtcNow;
           var customerData = CustomerMapper.SerializeCustomer(customer);
           var newCustomer = _customerService.CreateCustomer(customerData);
           return Ok(newCustomer);
        }

        [HttpGet("/api/customer")]
        public ActionResult GetCustomer()
        {
            _logger.LogInformation("Getting Customer");
            var customers = _customerService.GetAllCustomers();
            var customerModels = customers.Select(customers => new CustomerModel
                {
                    Id = customers.Id,
                    FirstName = customers.FirstName,
                    LastName = customers.LastName,
                    PrimaryAddress = CustomerMapper.MapCustomerAddress(customers.PrimaryAddress),
                    CreatedOn = customers.CreatedOn,
                    UpdatedOn = customers.UpdatedOn
                })
                .OrderByDescending(customer => customer.CreatedOn)
                .ToList();
            return Ok(customerModels);
        }

        [HttpDelete("/api/customer/{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            _logger.LogInformation("Deleting a customer");
            var response = _customerService.DeleteCustomer(id);
            return Ok(response);
        }
    }
}