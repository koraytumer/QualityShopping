﻿using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityShopping.WebUI.Models
{
    public class CategoryListViewModel
    {
        public List<Category> Categories { get; set; }
        public string SelectedCategory { get; set; }
    }
}
