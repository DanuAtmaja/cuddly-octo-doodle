using System;
using System.Collections.Generic;

namespace SenjaCoffee.Web.ViewModels
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public CustomerModel Customer { get; set; }
        public List<SalesOrderItemModel> SalesOrderItem { get; set; }
        public bool IsPaid { get; set; }
    }
}