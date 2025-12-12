# Google Cloud Console Setup Guide

## 📋 Overview

Hướng dẫn chi tiết để cấu hình Google OAuth 2.0 trên Google Cloud Console.

## ✅ Your Current Setup

- **Client ID:** `827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com`
- **Creation Date:** June 6, 2025
- **Last Used:** December 3, 2025
- **Status:** Enabled

## 🔧 Step-by-Step Setup

### Step 1: Access Google Cloud Console

1. Go to https://console.cloud.google.com/
2. Sign in with your Google account
3. Select your project (or create a new one)

### Step 2: Enable Google+ API

1. In the left sidebar, click "APIs & Services"
2. Click "Library"
3. Search for "Google+ API"
4. Click on it
5. Click "Enable"

### Step 3: Create OAuth 2.0 Credentials

1. Go to "APIs & Services" → "Credentials"
2. Click "Create Credentials" → "OAuth 2.0 Client ID"
3. If prompted, configure OAuth consent screen first:
   - Choose "External" user type
   - Fill in app name, user support email, developer contact
   - Add scopes: `email`, `profile`, `openid`
   - Add test users if needed

### Step 4: Configure OAuth Client

1. Choose "Web application" as application type
2. Name: "Cultural E-commerce Frontend" (or your app name)
3. Add Authorized JavaScript origins:
   ```
   http://localhost:5173
   http://localhost:3000
   https://yourdomain.com
   ```
4. Add Authorized redirect URIs:
   ```
   http://localhost:5173
   http://localhost:3000
   https://yourdomain.com
   ```
5. Click "Create"

### Step 5: Copy Client ID

1. After creation, you'll see your Client ID
2. Copy it: `827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com`
3. Add to `.env` file:
   ```env
   VITE_GOOGLE_CLIENT_ID=827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com
   ```

## 🌐 Authorized Origins & Redirect URIs

### Development
```
http://localhost:5173
http://localhost:3000
```

### Production
```
https://yourdomain.com
https://www.yourdomain.com
```

**Note:** It may take 5 minutes to a few hours for settings to take effect.

## 🔐 Client Secret Management

### Important Security Notes

1. **Never commit secrets to code**
   - Client secrets should only be used on backend
   - Frontend only uses Client ID

2. **Rotate secrets regularly**
   - Go to Credentials page
   - Click on your OAuth client
   - Click "Edit"
   - Under "Client secrets", click "Rotate"

3. **Store securely**
   - Use environment variables
   - Use secrets management service
   - Never share publicly

### For Backend

If backend needs to verify tokens:

1. Go to Credentials page
2. Click on your OAuth client
3. Under "Client secrets", you'll see your secret
4. Add to backend `.env`:
   ```env
   GOOGLE_CLIENT_SECRET=your_secret_here
   ```

## 📝 OAuth Consent Screen

### Configure Consent Screen

1. Go to "APIs & Services" → "OAuth consent screen"
2. Choose "External" user type
3. Fill in required information:
   - **App name:** Cultural E-commerce
   - **User support email:** support@yourdomain.com
   - **Developer contact:** developer@yourdomain.com

4. Add scopes:
   - `email`
   - `profile`
   - `openid`

5. Add test users (for development):
   - Your email
   - Team members' emails

6. Review and publish

## 🧪 Testing

### Test OAuth Flow

1. Go to your app: `http://localhost:5173`
2. Click "Đăng nhập với Google"
3. Google OAuth popup should appear
4. Authenticate with your Google account
5. You should be redirected back to your app

### Troubleshooting

**Issue: "Redirect URI mismatch"**
- Solution: Add the exact URL to Authorized redirect URIs
- Include protocol (http/https)
- Include port number if applicable

**Issue: "Invalid Client ID"**
- Solution: Verify Client ID in .env matches Google Cloud Console
- Check for typos
- Ensure it's the correct project

**Issue: "Consent screen not configured"**
- Solution: Go to OAuth consent screen and configure it
- Add required information
- Publish the consent screen

## 📊 Monitoring

### View OAuth Activity

1. Go to "APIs & Services" → "Credentials"
2. Click on your OAuth client
3. View:
   - Creation date
   - Last used date
   - Authorized origins
   - Redirect URIs

### Check API Usage

1. Go to "APIs & Services" → "Library"
2. Search for "Google+ API"
3. Click on it
4. View usage statistics

## 🔄 Updating Configuration

### Add New Domain

1. Go to "APIs & Services" → "Credentials"
2. Click on your OAuth client
3. Click "Edit"
4. Add new domain to:
   - Authorized JavaScript origins
   - Authorized redirect URIs
5. Click "Save"

### Change Consent Screen

1. Go to "OAuth consent screen"
2. Click "Edit"
3. Update information
4. Click "Save and Continue"

## 🚀 Production Deployment

### Before Going Live

1. ✅ Test OAuth flow thoroughly
2. ✅ Add production domain to authorized origins
3. ✅ Update .env with production values
4. ✅ Configure HTTPS
5. ✅ Set up error monitoring
6. ✅ Test token refresh
7. ✅ Monitor authentication logs

### Production Checklist

- [ ] Production domain added to Google Cloud Console
- [ ] HTTPS configured
- [ ] .env updated with production values
- [ ] Backend token verification implemented
- [ ] Error handling in place
- [ ] Logging configured
- [ ] Rate limiting implemented
- [ ] CORS policy configured
- [ ] Security headers set
- [ ] Monitoring and alerts set up

## 📞 Support

### Google OAuth Documentation
- https://developers.google.com/identity/protocols/oauth2
- https://developers.google.com/identity/gsi/web

### Google Cloud Console Help
- https://cloud.google.com/docs

### Common Issues
- https://developers.google.com/identity/gsi/web/guides/troubleshoot

## 🎯 Next Steps

1. ✅ Configure Google Cloud Console (Done)
2. ✅ Add Client ID to .env (Done)
3. ✅ Implement frontend (Done)
4. 🔄 Implement backend
5. 🔄 Test full flow
6. 🔄 Deploy to production

## 📋 Checklist

### Google Cloud Console
- [x] Project created
- [x] Google+ API enabled
- [x] OAuth 2.0 credentials created
- [x] Client ID obtained
- [x] Authorized origins configured
- [x] Redirect URIs configured
- [x] Consent screen configured

### Frontend
- [x] Client ID added to .env
- [x] @react-oauth/google installed
- [x] GoogleOAuthProvider configured
- [x] GoogleLoginButton implemented
- [x] LoginPage integrated
- [x] RegisterPage integrated

### Backend
- [ ] Google.Apis.Auth installed
- [ ] GoogleAuthService implemented
- [ ] Token verification implemented
- [ ] /login/google endpoint created
- [ ] User creation/update implemented
- [ ] JWT token generation implemented
- [ ] Error handling implemented
- [ ] Logging implemented

### Testing
- [ ] Frontend OAuth flow tested
- [ ] Backend token verification tested
- [ ] Full integration tested
- [ ] Error scenarios tested
- [ ] Production domain tested

## 🔐 Security Reminders

1. **Never expose Client Secret**
   - Only use on backend
   - Never commit to code

2. **Always use HTTPS**
   - In production
   - For OAuth callbacks

3. **Validate tokens**
   - Backend must verify Google tokens
   - Check token expiration
   - Validate audience (Client ID)

4. **Secure token storage**
   - Store refresh tokens securely
   - Hash before storing
   - Implement token rotation

5. **Monitor activity**
   - Log authentication events
   - Monitor failed attempts
   - Set up alerts

---

**Last Updated:** December 11, 2025
**Status:** ✅ Frontend Ready | 🔄 Backend In Progress
