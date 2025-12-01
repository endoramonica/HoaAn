using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Seeds;

namespace VietCommerce.Data.Seeders
{
    public static class PermissionSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1️⃣ Khởi tạo danh sách permission
            var permissions = new List<Permission>
            {
                // Product
                new Permission { Id = Guid.NewGuid(), Name = "product.view", Description = "Xem sản phẩm" },
                new Permission { Id = Guid.NewGuid(), Name = "product.create", Description = "Tạo sản phẩm" },
                new Permission { Id = Guid.NewGuid(), Name = "product.update", Description = "Cập nhật sản phẩm" },
                new Permission { Id = Guid.NewGuid(), Name = "product.delete", Description = "Xóa sản phẩm" },

                // Order
                new Permission { Id = Guid.NewGuid(), Name = "order.view", Description = "Xem đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "order.create", Description = "Tạo đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "order.update", Description = "Cập nhật đơn hàng" },

                // Customer
                new Permission { Id = Guid.NewGuid(), Name = "customer.view", Description = "Xem khách hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "customer.manage", Description = "Quản lý khách hàng" },

                // Inventory & Reports
                new Permission { Id = Guid.NewGuid(), Name = "report.view", Description = "Xem báo cáo" },

                // ADDRESS PERMISSIONS
                new Permission { Id = Guid.NewGuid(), Name = PermissionConstants.CustomerAddressManage, Description = "Quản lý địa chỉ cá nhân" },
                new Permission { Id = Guid.NewGuid(), Name = PermissionConstants.AdminAddressManage, Description = "Quản lý tất cả địa chỉ (admin)" },
                new Permission { Id = Guid.NewGuid(), Name = PermissionConstants.AdminAddressRead, Description = "Xem tất cả địa chỉ (admin)" },
          
                // Orders
                new Permission { Id = Guid.NewGuid(), Name = "orders.view_own", Description = "Xem đơn hàng của mình" },
                new Permission { Id = Guid.NewGuid(), Name = "orders.view_all", Description = "Xem tất cả đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "orders.create", Description = "Tạo đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "orders.update", Description = "Cập nhật đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "orders.delete", Description = "Xóa đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "orders.assign", Description = "Giao đơn hàng" },

                // POS
                new Permission { Id = Guid.NewGuid(), Name = "pos.access", Description = "Truy cập POS" },
                new Permission { Id = Guid.NewGuid(), Name = "pos.scan_barcode", Description = "Quét mã vạch" },
                new Permission { Id = Guid.NewGuid(), Name = "pos.process_payment", Description = "Xử lý thanh toán" },
                new Permission { Id = Guid.NewGuid(), Name = "pos.open_close_shift", Description = "Mở/Đóng ca POS" },

                // Inventory
                new Permission { Id = Guid.NewGuid(), Name = "inventory.view", Description = "Xem kho hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "inventory.manage", Description = "Quản lý kho hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "inventory.transfer", Description = "Chuyển kho" },
                new Permission { Id = Guid.NewGuid(), Name = "inventory.adjust", Description = "Điều chỉnh tồn kho" },

                // Staff Management
                new Permission { Id = Guid.NewGuid(), Name = "users.view_all", Description = "Xem tất cả nhân viên" },
                new Permission { Id = Guid.NewGuid(), Name = "users.manage", Description = "Quản lý nhân viên" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.assign", Description = "Giao việc" },
                new Permission { Id = Guid.NewGuid(), Name = "analytics.view_staff_reports", Description = "Xem báo cáo nhân viên" },

                // Tasks
                new Permission { Id = Guid.NewGuid(), Name = "tasks.view_own", Description = "Xem việc của mình" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.view_all", Description = "Xem tất cả việc" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.create", Description = "Tạo việc" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.update_own", Description = "Cập nhật việc của mình" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.update_all", Description = "Cập nhật tất cả việc" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.delete_own", Description = "Xóa việc của mình" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.delete_all", Description = "Xóa tất cả việc" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.complete", Description = "Hoàn thành việc" },
                new Permission { Id = Guid.NewGuid(), Name = "tasks.update_status", Description = "Cập nhật trạng thái việc" },

                // Analytics & Reports
                new Permission { Id = Guid.NewGuid(), Name = "analytics.view_store_dashboard", Description = "Xem dashboard cửa hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "analytics.export_reports", Description = "Xuất báo cáo" },
                new Permission { Id = Guid.NewGuid(), Name = "analytics.schedule_reports", Description = "Lên lịch báo cáo" },

                // CRM
                new Permission { Id = Guid.NewGuid(), Name = "crm.view", Description = "Xem CRM" },
                new Permission { Id = Guid.NewGuid(), Name = "crm.edit", Description = "Chỉnh sửa CRM" },
                new Permission { Id = Guid.NewGuid(), Name = "crm.create_interactions", Description = "Tạo tương tác CRM" },
                new Permission { Id = Guid.NewGuid(), Name = "crm.view_analytics", Description = "Xem phân tích CRM" },

                // LRM
                new Permission { Id = Guid.NewGuid(), Name = "lrm.view", Description = "Xem LRM" },
                new Permission { Id = Guid.NewGuid(), Name = "lrm.edit", Description = "Chỉnh sửa LRM" },
                new Permission { Id = Guid.NewGuid(), Name = "lrm.manage_suppliers", Description = "Quản lý nhà cung cấp" },
                new Permission { Id = Guid.NewGuid(), Name = "lrm.request_transfers", Description = "Yêu cầu chuyển kho" },
                new Permission { Id = Guid.NewGuid(), Name = "lrm.confirm_deliveries", Description = "Xác nhận giao hàng" },

                // HRM
                new Permission { Id = Guid.NewGuid(), Name = "hrm.view_own", Description = "Xem HRM của mình" },
                new Permission { Id = Guid.NewGuid(), Name = "hrm.view_all", Description = "Xem tất cả HRM" },
                new Permission { Id = Guid.NewGuid(), Name = "hrm.manage", Description = "Quản lý HRM" },
                new Permission { Id = Guid.NewGuid(), Name = "hrm.submit_leave_requests", Description = "Nộp đơn nghỉ phép" },
                new Permission { Id = Guid.NewGuid(), Name = "hrm.approve_leave_requests", Description = "Phê duyệt đơn nghỉ phép" },
                new Permission { Id = Guid.NewGuid(), Name = "hrm.assign_shifts", Description = "Phân ca làm việc" },

                // System
                new Permission { Id = Guid.NewGuid(), Name = "system.manage_settings", Description = "Quản lý cài đặt hệ thống" },
                new Permission { Id = Guid.NewGuid(), Name = "system.view_audit_log", Description = "Xem nhật ký kiểm tra" },
                new Permission { Id = Guid.NewGuid(), Name = "system.send_broadcast", Description = "Gửi broadcast" },

                // Payment 
                new Permission { Id = Guid.NewGuid(), Name = "payment.process", Description = "Xử lý thanh toán" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.process.cash", Description = "Xử lý thanh toán tiền mặt" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.process.card", Description = "Xử lý thanh toán thẻ" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.refund", Description = "Hoàn tiền thanh toán" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.void", Description = "Hủy thanh toán" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.calculate", Description = "Tính toán tổng đơn hàng" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.view.own", Description = "Xem lịch sử thanh toán của mình" },
                new Permission { Id = Guid.NewGuid(), Name = "payment.view.all", Description = "Xem toàn bộ lịch sử thanh toán" },
                // Admin-only
                new Permission { Id = Guid.NewGuid(), Name = "admin.manage_users", Description = "Quản lý người dùng (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.manage_roles", Description = "Quản lý vai trò (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.manage_permissions", Description = "Quản lý quyền (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.system_configuration", Description = "Cấu hình hệ thống (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.database_backup", Description = "Sao lưu cơ sở dữ liệu (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.system_monitor", Description = "Giám sát hệ thống (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.manage_stores", Description = "Quản lý cửa hàng (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.view_all_analytics", Description = "Xem toàn bộ báo cáo (Admin)" },
                new Permission { Id = Guid.NewGuid(), Name = "admin.super_admin", Description = "Siêu quản trị viên (Admin)" },
                //Post
                new Permission { Id = Guid.NewGuid(), Name = "post:create", Description = "Tạo bài viết" },
                new Permission { Id = Guid.NewGuid(), Name = "post:view", Description = "Xem bài viết" },
                new Permission { Id = Guid.NewGuid(), Name = "post:update", Description = "Cập nhật bài viết" },
                new Permission { Id = Guid.NewGuid(), Name = "post:delete", Description = "Xóa bài viết" },



            };

            // Add Cart and Order related permissions
            var cartOrderPerms = CartOrderPermissionSeed.GetCartOrderPermissions();
            permissions.AddRange(cartOrderPerms);

            // 2️⃣ Lọc những permission chưa có trong DB
            var existingNames = await context.Permissions.Select(p => p.Name).ToListAsync();
            var permissionsToAdd = permissions.Where(p => !existingNames.Contains(p.Name)).ToList();

            // 3️⃣ Thêm mới nếu có
            if (permissionsToAdd.Any())
            {
                await context.Permissions.AddRangeAsync(permissionsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}
