// File: VietCommerce.Core/Common/Constants/PermissionConstants.cs
namespace VietCommerce.Core.Common.Constants
{
    public static class PermissionConstants
    {
        // ==========================
        // Address permissions
        // ==========================
        public const string CustomerAddressManage = "customer.address.manage";  // Quản lý địa chỉ cá nhân
        public const string AdminAddressManage = "admin.address.manage";        // Quản lý tất cả địa chỉ (admin)
        public const string AdminAddressRead = "admin.address.read";            // Xem tất cả địa chỉ (admin)

        // ==========================
        // Cart permissions
        // ==========================
        public const string CartView = "cart.view";        // Xem giỏ hàng cá nhân
        public const string CartManage = "cart.manage";    // Thêm / xóa / cập nhật giỏ hàng cá nhân

        // ==========================
        // Order permissions
        // ==========================
        public const string OrderViewOwn = "order.view.own";       // Xem đơn hàng của chính mình
        public const string OrderCreate = "order.create";         // Tạo đơn hàng
        public const string OrderCancelOwn = "order.cancel.own";  // Hủy đơn hàng của chính mình

        public const string OrderViewAll = "order.view.all";           // Xem tất cả đơn hàng (admin/store manager)
        public const string OrderUpdateStatus = "order.update.status"; // Cập nhật trạng thái đơn hàng (admin/store manager)
    }
}
