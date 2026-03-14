using ComplaintManagementSystem.Shared.DTOs;
using Dapper;
using ComplaintManagementSystem.API.Data;

namespace ComplaintManagementSystem.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = "SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate FROM Categories ORDER BY CategoryName";
            return await conn.QueryAsync<CategoryResponse>(sql);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int categoryId)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = "SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate FROM Categories WHERE CategoryID = @Id";
            return await conn.QuerySingleOrDefaultAsync<CategoryResponse>(sql, new { Id = categoryId });
        }

        public async Task<int> CreateAsync(CategoryRequest request)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Categories (CategoryName, Description, IsActive, CreatedDate) 
                        VALUES (@CategoryName, @Description, @IsActive, GETUTCDATE());
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            return await conn.ExecuteScalarAsync<int>(sql, request);
        }

        public async Task<bool> UpdateAsync(int categoryId, CategoryRequest request)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Categories 
                        SET CategoryName = @CategoryName, Description = @Description, IsActive = @IsActive 
                        WHERE CategoryID = @Id";
            var affected = await conn.ExecuteAsync(sql, new { 
                Id = categoryId, 
                CategoryName = request.CategoryName, 
                Description = request.Description, 
                IsActive = request.IsActive 
            });
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Categories WHERE CategoryID = @Id";
            var affected = await conn.ExecuteAsync(sql, new { Id = categoryId });
            return affected > 0;
        }
    }
}