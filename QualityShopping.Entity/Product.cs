using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QualityShopping.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
