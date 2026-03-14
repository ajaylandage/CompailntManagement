using ComplaintManagementSystem.Shared.Entities;
using Dapper;
using ComplaintManagementSystem.API.Data;

namespace ComplaintManagementSystem.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AuthRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, PasswordHash, RoleID, IsActive, CreatedDate)
                        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @RoleId, @IsActive, @CreatedDate);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = await conn.ExecuteScalarAsync<int>(sql, user);
            return id;
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = "SELECT UserID AS Id, FirstName, LastName, Email, PhoneNumber, PasswordHash, RoleID AS RoleId, IsActive, CreatedDate FROM Users WHERE Email = @u OR PhoneNumber = @u";
            return await conn.QuerySingleOrDefaultAsync<User>(sql, new { u = usernameOrEmail });
        }
    }
}