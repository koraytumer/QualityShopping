using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QualityShopping.DataAccess.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Category GetByIdWithProducts(int id);
        void DeleteFromCategory(int categoryId, int productId);
    }
}
