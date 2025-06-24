using blogpost_website_api.DB;
using blogpost_website_api.DTO;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class CategoryService
    {
        private readonly IMongoCollection<CategoryEntity> _category;
        private readonly IMongoCollection<PostEntity> _post;

        public CategoryService(MongoDBContext context)
        {
            _category = context.GetCollection<CategoryEntity>("Category");
            _post = context.GetCollection<PostEntity>("Post");
        }
        // Create new category
        public async Task<respons> CreateCategory(CategoryDTO categoryDTO)
        {
            var existing = await _category.Find(c => c.Name.ToLower() == categoryDTO.Name.ToLower()).FirstOrDefaultAsync();
            if (existing != null)
                return new respons(false, "Category name already exists");

            var category = new CategoryEntity
            {
                Name = categoryDTO.Name,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                PostID = new List<string>()
            };

            await _category.InsertOneAsync(category);
            return new respons(true, "Category successfully created");
        }

        // Get all categories
        public async Task<respons> GetAllCategories()
        {
            var categories = await _category.Find(_ => true).ToListAsync();
            return categories.Count > 0
                ? new respons(true, categories)
                : new respons(false, "No categories found");
        }

        // Get category by ID
        public async Task<respons> GetCategoryById(string categoryId)
        {
            var category = await _category.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            return category != null
                ? new respons(true, category)
                : new respons(false, "Category not found");
        }

        // Update category
        public async Task<respons> UpdateCategory(string categoryId, CategoryDTO categoryDTO)
        {
            var existing = await _category.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            if (existing == null)
                return new respons(false, "Category not found");

            var update = Builders<CategoryEntity>.Update
                .Set(c => c.Name, categoryDTO.Name)
                .Set(c => c.UpdateAt,DateTime.UtcNow);
            var result = await _category.UpdateOneAsync(c => c.Id == categoryId, update);

            return result.ModifiedCount > 0
                ? new respons(true, "Category updated successfully")
                : new respons(false, "Update failed");
        }

        // Delete category and remove its ID from posts
        public async Task<respons> DeleteCategory(string categoryId)
        {
            // Remove category ID from all posts
            var postFilter = Builders<PostEntity>.Filter.Eq(p => p.CategoryID, categoryId);
            var postUpdate = Builders<PostEntity>.Update.Set(p => p.CategoryID, null);
            await _post.UpdateManyAsync(postFilter, postUpdate);

            // Delete category
            var result = await _category.DeleteOneAsync(c => c.Id == categoryId);
            return result.DeletedCount > 0
                ? new respons(true, "Category deleted successfully")
                : new respons(false, "Category not found");
        }

        // Search category
        public async Task<respons> SearchCategories(string text)
        {
            FilterDefinition<CategoryEntity> filter;

            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "#")
            {
                filter = Builders<CategoryEntity>.Filter.Empty;
            }
            else
            {
                filter = Builders<CategoryEntity>.Filter.Regex(
                    c => c.Name,
                    new MongoDB.Bson.BsonRegularExpression(text, "i")
                );
            }

            var categories = await _category.Find(filter).ToListAsync();
            return categories.Count > 0
                ? new respons(true, categories)
                : new respons(false, "No matching categories found");
        }
    }
}
