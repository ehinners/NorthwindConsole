using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NorthwindConsole.Model
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        [Required]
        [MaxLength(40)]
        public string ProductName { get; set; }
        [Range(0, int.MaxValue)]
        public int? SupplierId { get; set; }
        [Range(0, int.MaxValue)]
        public int? CategoryId { get; set; }
        [MaxLength(20)]
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        [Range(0, short.MaxValue)]
        public short? UnitsInStock { get; set; }
        [Range(0, short.MaxValue)]
        public short? UnitsOnOrder { get; set; }
        [Range(0, short.MaxValue)]
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
