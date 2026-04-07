
using AutoMapper;
using AutoMapper.QueryableExtensions;
using b1.Data;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace b1.ToDo
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        public CategoryService(AppDbContext appDbContext, IMapper mapper, IDistributedCache cache)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _cache = cache;

        }
        public async Task<Category> AddCategoryAsync(CategoryCreateDto CateDto)
        {
            
            var newCategory = _mapper.Map<Category>(CateDto);
            _appDbContext.Categories.Add(newCategory);
            await _appDbContext.SaveChangesAsync();
            await _cache.RemoveAsync("all_categories");
            return newCategory;
        }

        public async Task<bool> CategoryExistsAsync(int? CategoryID)
        {
            if(CategoryID == null)
            {
                throw new ArgumentException("CategoryID không được để trống");
            }
            return await _appDbContext.Categories.AnyAsync(c => c.Id == CategoryID);
        }

        public async Task DeleteCategoryAsync(int Id)
        {
            var Category = await _appDbContext.Categories.FindAsync(Id);
            if (Category == null)
            {
                throw new ArgumentException("lỗi");
            }
            Category.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();
            await _cache.RemoveAsync("all_categories");
        }

        public async Task<List<CategoryGetDto>> GetAllCategoriesAsync()
        {
            return await GetCachedDataAsync("all_categories_dto", async () =>
            {
                return await _appDbContext.Categories
                .Where(c => !c.IsDeleted)
                .ProjectTo<CategoryGetDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            });
            
        }


        public async Task<List<Category>> GetAllCategoriesNoTodoAsync()
        {
            return await GetCachedDataAsync("all_categories_entity", async () =>
            {
                return await _appDbContext.Categories.Where(c => !c.IsDeleted).ToListAsync();
            });
     
        }
        private async Task<T> GetCachedDataAsync<T>(string cacheKey, Func<Task<T>> dbQuery)
        {
            // 1. Kiểm tra Cache
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<T>(cachedData);
            }

            // 2. Nếu không có, gọi Database (thực thi cái dbQuery bạn truyền vào)
            var data = await dbQuery();

            // 3. Lưu vào Cache (10 phút)
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options);

            return data;
        }
    }
}
