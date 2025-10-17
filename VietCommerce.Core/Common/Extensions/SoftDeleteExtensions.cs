using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Tasks;



namespace VietCommerce.Core.Common.Extensions
{
    
        public static class SoftDeleteExtensions
        {
            // ========================
            // Sync: Xóa mềm / Phục hồi 1 entity
            // ========================
            public static void SoftDelete(this ISoftDelete entity, Guid userId)
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));

                entity.IsDeleted = true;
                entity.IsActive = false;
                entity.DeletedAt = DateTime.UtcNow;
                entity.DeletedBy = userId;
            }

            public static void Restore(this ISoftDelete entity)
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));

                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.DeletedAt = null;
                entity.DeletedBy = null;
            }

            // ========================
            // Sync: Xóa mềm / Phục hồi nhiều entity
            // ========================
            public static void SoftDeleteAll(this IEnumerable<ISoftDelete> entities, Guid userId)
            {
                if (entities == null) throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    entity.SoftDelete(userId);
            }

            public static void RestoreAll(this IEnumerable<ISoftDelete> entities)
            {
                if (entities == null) throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    entity.Restore();
            }

            // ========================
            // Sync: LINQ-friendly theo điều kiện
            // ========================
            public static void SoftDeleteWhere(this IEnumerable<ISoftDelete> entities, Func<ISoftDelete, bool> predicate, Guid userId)
            {
                if (entities == null) throw new ArgumentNullException(nameof(entities));
                if (predicate == null) throw new ArgumentNullException(nameof(predicate));

                foreach (var entity in entities.Where(predicate))
                    entity.SoftDelete(userId);
            }

            public static void RestoreWhere(this IEnumerable<ISoftDelete> entities, Func<ISoftDelete, bool> predicate)
            {
                if (entities == null) throw new ArgumentNullException(nameof(entities));
                if (predicate == null) throw new ArgumentNullException(nameof(predicate));

                foreach (var entity in entities.Where(predicate))
                    entity.Restore();
            }

            // ========================
            // Async: Xóa mềm / Phục hồi IQueryable (Entity Framework)
            // ========================
            public static async Task SoftDeleteAllAsync<T>(this IQueryable<T> query, Guid userId)
                where T : class, ISoftDelete
            {
                if (query == null) throw new ArgumentNullException(nameof(query));

                var entities = await query.ToListAsync();
                foreach (var entity in entities)
                    entity.SoftDelete(userId);
            }

            public static async Task RestoreAllAsync<T>(this IQueryable<T> query)
                where T : class, ISoftDelete
            {
                if (query == null) throw new ArgumentNullException(nameof(query));

                var entities = await query.ToListAsync();
                foreach (var entity in entities)
                    entity.Restore();
            }

            
        }
    }
