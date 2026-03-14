using ComplaintManagementSystem.Shared.Entities;
using ComplaintManagementSystem.Shared.DTOs;
using Dapper;
using ComplaintManagementSystem.API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ComplaintManagementSystem.Shared.Enums;
using System.Linq;
using System.Text;

namespace ComplaintManagementSystem.API.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ComplaintRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Complaint complaint)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Complaints (UserID, CategoryID, StatusID, Title, Description, LocationAddress, Priority, Latitude, Longitude, CreatedDate)
                        VALUES (@UserId, @CategoryId, @StatusId, @Title, @Description, @LocationAddress, @Priority, @Latitude, @Longitude, @CreatedAt);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = await conn.ExecuteScalarAsync<int>(sql, complaint);
            return id;
        }

        public async Task<IEnumerable<ComplaintListResponse>> GetAllAsync(int? statusId = null, int? categoryId = null, string? priority = null, string? search = null, int? userId = null)
        {
            using var conn = _connectionFactory.CreateConnection();

            var sb = new StringBuilder();
            sb.Append(@"SELECT c.ComplaintID AS Id,
                       c.Title,
                       cat.CategoryName AS CategoryName,
                       s.StatusName AS StatusName,
                       c.Priority,
                       c.CreatedDate AS CreatedAt,
                       c.UserID AS UserId
                FROM Complaints c
                LEFT JOIN Categories cat ON c.CategoryID = cat.CategoryID
                LEFT JOIN ComplaintStatuses s ON c.StatusID = s.StatusID");

            sb.Append(" WHERE 1=1");

            var parameters = new DynamicParameters();

            if (userId.HasValue)
            {
                // show complaints created by the user OR currently assigned to them
                sb.Append(" AND (c.UserID = @UserId OR c.AssignedToUserID = @UserId)");
                parameters.Add("UserId", userId.Value);
            }
            if (statusId.HasValue)
            {
                sb.Append(" AND c.StatusID = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }
            if (categoryId.HasValue)
            {
                sb.Append(" AND c.CategoryID = @CategoryId");
                parameters.Add("CategoryId", categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(priority))
            {
                sb.Append(" AND c.Priority = @Priority");
                parameters.Add("Priority", priority);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                sb.Append(" AND c.Title LIKE '%' + @Search + '%'");
                parameters.Add("Search", search);
            }

            sb.Append(" ORDER BY c.CreatedDate DESC");

            var sql = sb.ToString();

            return await conn.QueryAsync<ComplaintListResponse>(sql, parameters);
        }

        public async Task<ComplaintDetailsResponse?> GetByIdAsync(int id)
        {
            using var conn = _connectionFactory.CreateConnection();
            var sql = @"SELECT c.ComplaintID AS Id,
                               c.UserID AS UserId,
                               c.CategoryID AS CategoryId,
                               cat.CategoryName AS CategoryName,
                               c.StatusID AS StatusId,
                               s.StatusName AS StatusName,
                               c.Title,
                               c.Description,
                               c.LocationAddress,
                               c.Priority,
                               c.Latitude,
                               c.Longitude,
                               c.CreatedDate AS CreatedAt,
                               c.ResolvedDate AS ResolvedAt
                        FROM Complaints c
                        LEFT JOIN Categories cat ON c.CategoryID = cat.CategoryID
                        LEFT JOIN ComplaintStatuses s ON c.StatusID = s.StatusID
                        WHERE c.ComplaintID = @Id";

            var details = await conn.QuerySingleOrDefaultAsync<ComplaintDetailsResponse>(sql, new { Id = id });
            if (details == null) return null;

            var updatesSql = @"SELECT UpdateID AS Id,
                                       ComplaintID AS ComplaintId,
                                       UpdatedByUserID AS UpdatedByUserId,
                                       StatusID AS StatusId,
                                       UpdateMessage AS UpdateMessage,
                                       CreatedDate AS CreatedDate
                                FROM ComplaintUpdates
                                WHERE ComplaintID = @Id
                                ORDER BY CreatedDate";

            var attachmentsSql = @"SELECT AttachmentID AS Id,
                                           ComplaintID AS ComplaintId,
                                           FileName,
                                           FilePath,
                                           UploadedByUserID AS UploadedByUserId,
                                           CreatedDate AS UploadedAt
                                    FROM Attachments
                                    WHERE ComplaintID = @Id";

            var updates = (await conn.QueryAsync<ComplaintUpdateResponse>(updatesSql, new { Id = id })).AsList();
            var attachments = (await conn.QueryAsync<Attachment>(attachmentsSql, new { Id = id })).AsList();

            details.Updates = updates;
            details.Attachments = attachments;

            return details;
        }

        public async Task UpdateStatusAsync(int complaintId, int statusId, int updatedByUserId, string? note)
        {
            using var conn = _connectionFactory.CreateConnection();
            // Ensure connection is open before beginning a transaction
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            using var tran = conn.BeginTransaction();
            try
            {
                var updateSql = "UPDATE Complaints SET StatusID = @StatusId" +
                                ", ResolvedDate = CASE WHEN @StatusId = @ResolvedStatus THEN GETUTCDATE() ELSE ResolvedDate END" +
                                " WHERE ComplaintID = @Id";

                var resolvedStatus = (int)ComplaintStatusType.Resolved;

                await conn.ExecuteAsync(updateSql, new { StatusId = statusId, ResolvedStatus = resolvedStatus, Id = complaintId }, tran);

                var insertUpdateSql = @"INSERT INTO ComplaintUpdates (ComplaintID, StatusID, UpdateMessage, UpdatedByUserID, CreatedDate)
                                       VALUES (@ComplaintId, @StatusId, @Note, @UpdatedByUserId, GETUTCDATE());";

                await conn.ExecuteAsync(insertUpdateSql, new { ComplaintId = complaintId, StatusId = statusId, Note = note, UpdatedByUserId = updatedByUserId }, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task AssignAsync(ComplaintAssignment assignment)
        {
            using var conn = _connectionFactory.CreateConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }

            using var tran = conn.BeginTransaction();
            try
            {
                var deactivateSql = "UPDATE ComplaintAssignments SET IsActive = 0 WHERE ComplaintID = @ComplaintId";
                try
                {
                    await conn.ExecuteAsync(deactivateSql, new { ComplaintId = assignment.ComplaintId }, tran);
                }
                catch
                {
                    // ignore if schema doesn't have IsActive
                }

                var insertSql = @"INSERT INTO ComplaintAssignments (ComplaintID, AssignedToUserID, AssignedByUserID, AssignedDate)
                                  VALUES (@ComplaintId, @AssignedToUserId, @AssignedByUserId, @AssignedAt);";

                await conn.ExecuteAsync(insertSql, assignment, tran);

                // NEW: keep current assignee + last assignment date on master row
                var updateComplaintSql = @"UPDATE Complaints
                                           SET AssignedToUserID = @AssignedToUserId,
                                               LastAssignmentDate = @AssignedAt
                                           WHERE ComplaintID = @ComplaintId";
                await conn.ExecuteAsync(updateComplaintSql, assignment, tran);

                var statusSql = "SELECT StatusID FROM Complaints WHERE ComplaintID = @Id";
                var currentStatusId = await conn.QuerySingleOrDefaultAsync<int>(statusSql, new { Id = assignment.ComplaintId }, tran);

                var insertUpdateSql = @"INSERT INTO ComplaintUpdates (ComplaintID, StatusID, UpdateMessage, UpdatedByUserID, CreatedDate)
                                       VALUES (@ComplaintId, @StatusId, @UpdateMessage, @UpdatedByUserId, GETUTCDATE());";

                var updateMessage = $"Assigned to user {assignment.AssignedToUserId}";

                await conn.ExecuteAsync(
                    insertUpdateSql,
                    new
                    {
                        ComplaintId = assignment.ComplaintId,
                        StatusId = currentStatusId,
                        UpdateMessage = updateMessage,
                        UpdatedByUserId = assignment.AssignedByUserId
                    },
                    tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<DashboardResponse> GetDashboardDataAsync(int? userId = null)
        {
            using var conn = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            var whereClause = userId.HasValue ? "WHERE UserID = @UserId" : "";
            if (userId.HasValue)
            {
                parameters.Add("UserId", userId.Value);
            }

            // Use ISNULL to handle NULL values from SUM when there are no records
            var countsSql = $@"SELECT 
                                    ISNULL(COUNT(*), 0) AS Total,
                                    ISNULL(SUM(CASE WHEN StatusID = 1 THEN 1 ELSE 0 END), 0) AS Pending,
                                    ISNULL(SUM(CASE WHEN StatusID = 3 THEN 1 ELSE 0 END), 0) AS InProgress,
                                    ISNULL(SUM(CASE WHEN StatusID = 4 THEN 1 ELSE 0 END), 0) AS Resolved
                                FROM Complaints {whereClause}";

            var counts = await conn.QuerySingleOrDefaultAsync<DashboardCounts>(countsSql, parameters);

            var recentSql = $@"SELECT TOP 5 c.ComplaintID AS ComplaintId,
                                       c.Title,
                                       cat.CategoryName AS CategoryName,
                                       s.StatusName AS StatusName,
                                       c.CreatedDate AS CreatedDate
                               FROM Complaints c
                               LEFT JOIN Categories cat ON c.CategoryID = cat.CategoryID
                               LEFT JOIN ComplaintStatuses s ON c.StatusID = s.StatusID
                               {(userId.HasValue ? "WHERE c.UserID = @UserId" : "")}
                               ORDER BY c.CreatedDate DESC";

            var recent = (await conn.QueryAsync<RecentComplaintDto>(recentSql, parameters)).ToList();

            return new DashboardResponse
            {
                TotalComplaints = counts?.Total ?? 0,
                PendingComplaints = counts?.Pending ?? 0,
                InProgressComplaints = counts?.InProgress ?? 0,
                ResolvedComplaints = counts?.Resolved ?? 0,
                RecentComplaints = recent
            };
        }

        // Helper class for strongly-typed dashboard counts
        private class DashboardCounts
        {
            public int Total { get; set; }
            public int Pending { get; set; }
            public int InProgress { get; set; }
            public int Resolved { get; set; }
        }
    }
}