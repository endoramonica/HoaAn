namespace VietCommerce.Core.Enums.Notifications
{
    public enum NotificationType
    {
        // ===== SOCIAL NOTIFICATIONS =====
        PostLiked = 100,
        PostCommented = 101,
        CommentReplied = 102,
        PostShared = 103,
        PostMentioned = 104,

        FollowRequest = 110,
        FollowAccepted = 111,
        NewFollower = 112,

        MessageReceived = 120,
        MessageRead = 121,

        FriendRequest = 130,
        FriendAccepted = 131,

        // ===== E-COMMERCE NOTIFICATIONS =====
        OrderCreated = 200,
        OrderConfirmed = 201,
        OrderPaid = 202,
        OrderShipping = 203,
        OrderDelivered = 204,
        OrderCancelled = 205,
        OrderRefunded = 206,

        PaymentSuccess = 210,
        PaymentFailed = 211,
        PaymentPending = 212,

        ProductBackInStock = 220,
        ProductPriceDropped = 221,

        CartAbandoned = 230,
        WishlistItemOnSale = 231,

        // ===== LOYALTY & PROMOTION =====
        PointsEarned = 300,
        PointsExpiring = 301,
        TierUpgraded = 302,

        CouponReceived = 310,
        CouponExpiring = 311,

        FlashSaleStarting = 320,
        SpecialOffer = 321,

        // ===== ADMIN/SYSTEM NOTIFICATIONS =====
        AccountVerified = 400,
        PasswordChanged = 401,
        SecurityAlert = 402,

        LowStock = 500,
        OrderAssigned = 501,
        CustomerComplaint = 502,
        ReviewPosted = 503,

        SystemMaintenance = 600,
        SystemUpdate = 601,
        Announcement = 602,
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        SUCCESS = 4,
        ALERT = 5,
        OrderUpdate = 6   // b? sung
    }
}
