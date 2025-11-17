using Microsoft.Extensions.Logging;

namespace VietCommerce.Application.Helpers
{


    public static class SessionIdLoggerExtensions
    {
        public static void LogSessionExtracted(this ILogger logger, string sessionId)
        {
            logger.LogInformation("✅ Session ID extracted: {SessionId}",
                SessionIdHelper.FormatSessionIdForLogging(sessionId));
        }

        public static void LogSessionGenerated(this ILogger logger, string sessionId)
        {
            logger.LogInformation("✅ Session ID generated: {SessionId}",
                SessionIdHelper.FormatSessionIdForLogging(sessionId));
        }

        public static void LogSessionNotFound(this ILogger logger)
        {
            logger.LogWarning("⚠️ No session ID found in request");
        }

        public static void LogSessionCleared(this ILogger logger)
        {
            logger.LogInformation("✅ Session ID cleared");
        }
    }
}