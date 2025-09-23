using System;
using VietCommerce.Core.Common;

namespace VietCommerce.Core.Common.Extensions
{
    public static class EntityExtensions
    {
        public static bool IsDeleted(this ISoftDelete entity)
        {
            return entity.IsDeleted;
        }
        
        public static void MarkAsDeleted(this ISoftDelete entity)
        {
            entity.IsDeleted = true;
        }
        
        public static void SetAuditFields(this AuditableEntity entity, Guid userId, bool isNew = false)
        {
            if (isNew)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedBy = userId;
            }

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId;


        }
    }
}
