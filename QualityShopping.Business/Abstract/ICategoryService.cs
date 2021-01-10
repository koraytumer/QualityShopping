﻿using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QualityShopping.Business.Abstract
{
    public interface ICategoryService
    {
        List<Category> GetAll();
        Category GetByIdWithProducts(int id);
        Category GetById(int id);
        void Create(Category entity);
        void Update(Category entity);
        void Delete(Category entity);
        void DeleteFromCategory(int categoryId, int productId);
    }
}
