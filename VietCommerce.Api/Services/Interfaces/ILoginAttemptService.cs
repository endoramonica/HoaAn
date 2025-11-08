namespace VietCommerce.Api.Services.Interfaces
{
    public interface ILoginAttemptService
    {
        /// <summary>
        /// Kiểm tra xem tài khoản có đang bị khóa không
        /// </summary>
        /// <param name="email">Email người dùng</param>
        /// <returns>
        /// Tuple: isLocked = true nếu đang khóa, message = thông báo cho user
        /// </returns>
        Task<(bool isLocked, string message)> IsLockedAsync(string email);

        /// <summary>
        /// Tăng số lần đăng nhập thất bại và cập nhật TTL dựa theo LockLevel
        /// </summary>
        /// <param name="email">Email người dùng</param>
        /// <returns>Số lần failed hiện tại</returns>
        Task<int> IncreaseFailedCountAsync(string email);

        /// <summary>
        /// Reset số lần thử đăng nhập khi login thành công
        /// </summary>
        /// <param name="email">Email người dùng</param>
        Task ResetAsync(string email);
    }
}
