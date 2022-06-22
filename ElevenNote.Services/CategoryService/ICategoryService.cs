using ElevenNote.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.CategoryService
{
    public interface ICategoryService
    {
        bool CreateCategory(CategoryCreate model);
        IEnumerable<CategoryListItem> GetAllCategories();
        void SetUserId(Guid userId);
        CategoryDetail GetCategoryById(int id);
        bool UpdateCategory(CategoryEdit model);
        bool DeleteCategory(int categoryId);
    }
}
