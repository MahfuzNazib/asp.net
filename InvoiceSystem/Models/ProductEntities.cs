using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceSystem.Models
{
    public class ProductEntities
    {
        [Required]
        public string ProductName { get; set; }

        [Required]
        public double Price { get; set; }

    }
}
