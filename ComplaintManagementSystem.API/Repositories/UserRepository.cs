using ComplaintManagementSystem.Shared.Entities;
using Dapper;
using System.Data;
using ComplaintManagementSystem.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
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

        public async Task<User?> GetByIdAsync(int id)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT UserID AS Id,
                               FirstName,
                               LastName,
                               Email,
                               PhoneNumber,
                               PasswordHash,
                               RoleID AS RoleId,
                               IsActive,
                               CreatedDate
                        FROM Users
                        WHERE UserID = @Id";
            return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = "SELECT UserID AS Id, FirstName, LastName, Email, PhoneNumber, PasswordHash, RoleID AS RoleId, IsActive, CreatedDate FROM Users WHERE Email = @u OR PhoneNumber = @u";
            return await conn.QuerySingleOrDefaultAsync<User>(sql, new { u = usernameOrEmail });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = "SELECT UserID AS Id, FirstName, LastName, Email, PhoneNumber, RoleID AS RoleId FROM Users WHERE IsActive = 1";
            return await conn.QueryAsync<User>(sql);
        }
    }
}