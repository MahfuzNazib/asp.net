using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceSystem.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { set; get; }
        public string InvoiceDate { set; get; }
        public string InvoiceCode { set; get; }
        public double TotalAmount { set; get; }
        public double PaidAmount { set; get; }
        public double DueAmount { set; get; }

    }
}
