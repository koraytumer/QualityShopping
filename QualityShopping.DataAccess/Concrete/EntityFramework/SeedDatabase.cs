using Microsoft.EntityFrameworkCore;
using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QualityShopping.DataAccess.Concrete.EntityFramework
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();


            if (!context.Products.Any())
            {
                context.Products.AddRange(Products);
                context.AddRange(ProductCategory);

            }
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(Categories);

            }
            context.SaveChanges();
        }

        private static Category[] Categories = {
            new Category() { Name="Elektronik"},
            new Category() { Name="Moda"},
            new Category() { Name="Ev,Yaşam,Kırtasiye"}
        };

        private static Product[] Products =
        {
            new Product(){ Name="Samsung", Price=2000, ImageUrl="Phone1.jpg", Description="<p>Phone</p>"},
            new Product(){ Name="Asus", Price=3000, ImageUrl="Phone2.jpg", Description="<p>Phone</p>"},
            new Product(){ Name="Nexus", Price=4000, ImageUrl="Phone3.jpg", Description="<p>Phone</p>"}
        };

        private static ProductCategory[] ProductCategory =
        {
            new ProductCategory() { Product= Products[0],Category= Categories[0]},
            new ProductCategory() { Product= Products[1],Category= Categories[1]},
            new ProductCategory() { Product= Products[2],Category= Categories[2]}
        };
    }
}