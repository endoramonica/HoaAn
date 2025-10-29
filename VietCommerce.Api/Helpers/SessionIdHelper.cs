namespace VietCommerce.Api.Helpers
{
    public static class SessionIdHelper
    {// Constants
        private const string SESSION_ID_COOKIE_NAME = "sessionId";
        private const string SESSION_ID_HEADER_NAME = "X-Session-Id";
        private const string SESSION_ID_QUERY_PARAM = "sessionId";

        /// <summary>
        /// Extract session ID from HTTP request (cookie → header → query param)
        /// Returns null if not found
        /// </summary>
        public static string? ExtractSessionId(HttpRequest request, ILogger? logger = null)
        {
            try
            {
                // ✅ Priority 1: Try Cookie
                if (request.Cookies.TryGetValue(SESSION_ID_COOKIE_NAME, out var sessionIdFromCookie))
                {
                    if (IsValidSessionId(sessionIdFromCookie))
                    {
                        logger?.LogDebug("Session ID extracted from cookie: {SessionId}",
                            sessionIdFromCookie[..8] + "...");
                        return sessionIdFromCookie;
                    }
                }

                // ✅ Priority 2: Try Header
                if (request.Headers.TryGetValue(SESSION_ID_HEADER_NAME, out var sessionIdFromHeader))
                {
                    var headerValue = sessionIdFromHeader.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(headerValue) && IsValidSessionId(headerValue))
                    {
                        logger?.LogDebug("Session ID extracted from header: {SessionId}",
                            headerValue[..8] + "...");
                        return headerValue;
                    }
                }

                // ✅ Priority 3: Try Query Parameter
                if (request.Query.TryGetValue(SESSION_ID_QUERY_PARAM, out var sessionIdFromQuery))
                {
                    var queryValue = sessionIdFromQuery.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(queryValue) && IsValidSessionId(queryValue))
                    {
                        logger?.LogDebug("Session ID extracted from query param: {SessionId}",
                            queryValue[..8] + "...");
                        return queryValue;
                    }
                }

                logger?.LogDebug("No valid session ID found in request");
                return null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error extracting session ID from request");
                return null;
            }
        }

        /// <summary>
        /// Extract session ID from HttpContext (convenience method for controller use)
        /// </summary>
        public static string? ExtractSessionId(HttpContext httpContext, ILogger? logger = null)
        {
            if (httpContext?.Request == null)
            {
                logger?.LogWarning("HttpContext or HttpRequest is null");
                return null;
            }

            return ExtractSessionId(httpContext.Request, logger);
        }

        /// <summary>
        /// Set session ID in HTTP response (as secure HttpOnly cookie)
        /// </summary>
        public static void SetSessionIdCookie(HttpResponse response, string sessionId, int expirationDays = 30, ILogger? logger = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sessionId) || !IsValidSessionId(sessionId))
                {
                    logger?.LogWarning("Invalid session ID for cookie: {SessionId}", sessionId);
                    return;
                }

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,           // ✅ Prevent JS access (security)
                    Secure = true,             // ✅ HTTPS only
                    SameSite = SameSiteMode.Strict, // ✅ CSRF protection
                    Expires = DateTimeOffset.UtcNow.AddDays(expirationDays),
                    Path = "/"
                };

                response.Cookies.Append(SESSION_ID_COOKIE_NAME, sessionId, cookieOptions);

                logger?.LogInformation("Session ID cookie set: {SessionId}", sessionId[..8] + "...");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error setting session ID cookie");
            }
        }

        /// <summary>
        /// Generate new unique session ID (UUID v4)
        /// </summary>
        public static string GenerateSessionId()
        {
            return Guid.NewGuid().ToString("N"); // 32 chars, no hyphens
        }

        /// <summary>
        /// Generate and set session ID in response cookie
        /// </summary>
        public static string GenerateAndSetSessionId(HttpResponse response, int expirationDays = 30, ILogger? logger = null)
        {
            var newSessionId = GenerateSessionId();
            SetSessionIdCookie(response, newSessionId, expirationDays, logger);
            return newSessionId;
        }

        /// <summary>
        /// Clear session ID from cookies
        /// </summary>
        public static void ClearSessionId(HttpResponse response, ILogger? logger = null)
        {
            try
            {
                response.Cookies.Delete(SESSION_ID_COOKIE_NAME);
                logger?.LogInformation("Session ID cookie cleared");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error clearing session ID cookie");
            }
        }

        /// <summary>
        /// Validate session ID format (must be valid UUID)
        /// </summary>
        public static bool IsValidSessionId(string? sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return false;

            // Check if valid GUID format (with or without hyphens)
            return Guid.TryParse(sessionId, out _);
        }

        /// <summary>
        /// Get or extract session ID from request
        /// If not found, generates new one and sets in response
        /// </summary>
        public static string GetOrCreateSessionId(HttpContext httpContext, ILogger? logger = null)
        {
            try
            {
                // Try to extract existing session ID
                var existingSessionId = ExtractSessionId(httpContext, logger);
                if (!string.IsNullOrWhiteSpace(existingSessionId))
                {
                    logger?.LogDebug("Using existing session ID: {SessionId}", existingSessionId[..8] + "...");
                    return existingSessionId;
                }

                // Generate and set new session ID
                var newSessionId = GenerateAndSetSessionId(httpContext.Response, logger: logger);
                logger?.LogInformation("Generated and set new session ID: {SessionId}", newSessionId[..8] + "...");

                return newSessionId;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error getting or creating session ID");
                // Fallback: generate session ID in memory (not persisted in cookie)
                return GenerateSessionId();
            }
        }

        /// <summary>
        /// Refresh session ID expiration (extend cookie lifetime)
        /// </summary>
        public static string? RefreshSessionId(HttpContext httpContext, int expirationDays = 30, ILogger? logger = null)
        {
            try
            {
                var sessionId = ExtractSessionId(httpContext, logger);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    logger?.LogWarning("No session ID to refresh");
                    return null;
                }

                // Re-set cookie with new expiration
                SetSessionIdCookie(httpContext.Response, sessionId, expirationDays, logger);
                logger?.LogInformation("Session ID refreshed: {SessionId}", sessionId[..8] + "...");

                return sessionId;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error refreshing session ID");
                return null;
            }
        }

        /// <summary>
        /// Check if session ID exists and is valid in request
        /// </summary>
        public static bool HasValidSessionId(HttpRequest request)
        {
            var sessionId = ExtractSessionId(request);
            return !string.IsNullOrWhiteSpace(sessionId) && IsValidSessionId(sessionId);
        }

        /// <summary>
        /// Log session ID info (truncated for security)
        /// </summary>
        public static string FormatSessionIdForLogging(string? sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return "NULL";

            if (sessionId.Length <= 8)
                return sessionId;

            return $"{sessionId[..8]}...";
        }
    }
}
