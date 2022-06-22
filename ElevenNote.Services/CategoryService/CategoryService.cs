using ElevenNote.Data;
using ElevenNote.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private Guid _userId;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateCategory(CategoryCreate model)
        {
            var categoryEntity = new Category()
            {
                OwnerId = _userId,
                Name = model.Name,
            };

            _context.Categories.Add(categoryEntity);
            return _context.SaveChanges() == 1;
        }

        public bool DeleteCategory(int categoryId)
        {
            var entity = _context.Categories
                .SingleOrDefault(e => e.Id == categoryId && e.OwnerId == _userId);
            _context.Categories.Remove(entity);

            return _context.SaveChanges() == 1;
        }

        public IEnumerable<CategoryListItem> GetAllCategories()
        {
            var categories = _context.Categories
                .Where(e => e.OwnerId == _userId)
                .Select(e => 
                    new CategoryListItem()
                    {
                        CategoryId = e.Id,
                        Name = e.Name,
                        OwnwerId = e.OwnerId
                    }).ToList();
            return categories;
        }

        public CategoryDetail GetCategoryById(int id)
        {
            var category = _context.Categories
                .Single(e => e.Id == id && e.OwnerId == _userId);
            return new CategoryDetail()
            {
                CategoryId = category.Id,
                Name = category.Name,
            };
        }

        public bool UpdateCategory(CategoryEdit model)
        {
            var category = _context.Categories.Find(model.CategoryId);
            if (category?.OwnerId != _userId || category is null) return false;
                //.Single(e => e.Id == model.CategoryId && e.OwnerId == _userId);
            category.Name = model.Name;

            
            return _context.SaveChanges() == 1;
        }

        public void SetUserId(Guid userId) => _userId = userId;
    }
}
