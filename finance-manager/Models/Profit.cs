using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finance_manager.Models
{
    class Profit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TaxPercentage { get; set; }

        public decimal TaxAmount => Price * (TaxPercentage / 100);

        public string Date { get; set; }

        public Profit() { }

        public Profit(string name, decimal price, decimal taxPercentage)
        {
            Name = name;
            Price = price;
            TaxPercentage = taxPercentage;
            Date = DateTime.Now.ToString();
        }
    }
}
