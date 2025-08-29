using Supabase.Gotrue;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Storage;
using IMS_Mobile.MVVM.Models;
using System;
using System.Threading;
using System.Linq;
using System.Diagnostics;

namespace IMS_Mobile.Service
{
    public class SupabaseAuthService
    {
        #region Fields
        private readonly Supabase.Client _supabase;
        private UserSession? _currentSession;
        private bool _isOfflineSessionActive;
        #endregion

        public SupabaseAuthService(Supabase.Client supabaseClient)
        {
            _supabase = supabaseClient;
        }

        #region Public Properties
        public Supabase.Client GetClient() => _supabase;
        public bool IsUserAuthenticated => _currentSession != null && !_currentSession.IsExpired;
        public bool IsOfflineSessionActive => _isOfflineSessionActive;
        #endregion

        #region Authentication Methods
        public async Task<bool> InitializeAsync()
        {
            try
            {
                var accessToken = await SecureStorage.GetAsync("access_token");
                var refreshToken = await SecureStorage.GetAsync("refresh_token");

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    await _supabase.Auth.SetSession(accessToken, refreshToken);

                    var jwtToken = new JwtSecurityToken(accessToken);
                    _currentSession = new UserSession
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresAt = jwtToken.ValidTo
                    };

                    bool isValid = false;
                    if (NetworkHelper.IsConnected())
                    {
                        isValid = await ValidateSessionAsync();
                    }
                    else
                    {
                        isValid = _currentSession != null && !_currentSession.IsExpired;
                    }

                    _isOfflineSessionActive = !isValid;
                    if (isValid && NetworkHelper.IsConnected())
                    {
                        var userId = GetUserId();
                        LoggedIn?.Invoke(userId);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Auth init failed: {ex.Message}");
                return false;
            }
        }

        public async Task<UserSession?> SignInAsync(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignIn(email, password);
                if (session != null && !string.IsNullOrEmpty(session.AccessToken) && !string.IsNullOrEmpty(session.RefreshToken))
                {
                    var jwtToken = new JwtSecurityToken(session.AccessToken);
                    var userSession = new UserSession
                    {
                        AccessToken = session.AccessToken,
                        RefreshToken = session.RefreshToken,
                        ExpiresAt = jwtToken.ValidTo
                    };

                    await SecureStorage.SetAsync("access_token", session.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

                    _currentSession = userSession;
                    _isOfflineSessionActive = false;
                    LoggedIn?.Invoke(GetUserId());
                    return userSession;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign in failed: {ex.Message}");
                return null;
            }
        }

        public async Task<UserSession?> SignUpAsync(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignUp(email, password);
                if (session != null && !string.IsNullOrEmpty(session.AccessToken))
                {
                    var jwtToken = new JwtSecurityToken(session.AccessToken);
                    var userSession = new UserSession
                    {
                        AccessToken = session.AccessToken,
                        RefreshToken = session.RefreshToken,
                        ExpiresAt = jwtToken.ValidTo
                    };

                    await SecureStorage.SetAsync("access_token", session.AccessToken);
                    if (!string.IsNullOrEmpty(session.RefreshToken))
                        await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

                    _currentSession = userSession;
                    _isOfflineSessionActive = false;
                    LoggedIn?.Invoke(GetUserId());
                    return userSession;
                }

                var signedIn = await SignInAsync(email, password);
                return signedIn;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign up failed: {ex.Message}");
                return null;
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                await _supabase.Auth.SignOut();
                SecureStorage.Remove("access_token");
                SecureStorage.Remove("refresh_token");
                _currentSession = null;
                _isOfflineSessionActive = false;
                LoggedOut?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign out failed: {ex.Message}");
            }
        }
        #endregion

        #region Session Management
        public async Task<bool> ValidateSessionAsync()
        {
            try
            {
                var accessToken = await SecureStorage.GetAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                    return false;

                if (!NetworkHelper.IsConnected())
                    return false;

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        var user = await _supabase.Auth.GetUser(accessToken);
                        return user != null;
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Session validation failed: {ex.Message}");
                return false;
            }
        }

        public void HydrateOfflineSession(string accessToken, string refreshToken)
        {
            try
            {
                var jwtToken = new JwtSecurityToken(accessToken);
                _currentSession = new UserSession
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = jwtToken.ValidTo
                };
                _isOfflineSessionActive = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Offline session hydration failed: {ex.Message}");
            }
        }
        #endregion

        #region User Information
        public string GetUserId()
        {
            try
            {
                var currentUser = _supabase.Auth.CurrentUser;
                if (currentUser != null && !string.IsNullOrEmpty(currentUser.Id))
                    return currentUser.Id;

                if (_currentSession?.AccessToken is string token && !string.IsNullOrEmpty(token))
                {
                    var jwt = new JwtSecurityToken(token);
                    var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    return sub ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetUserId failed: {ex.Message}");
            }
            return string.Empty;
        }

        public Guid GetUserIdGuid()
        {
            var userIdString = GetUserId();
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid userId))
                return userId;
            return Guid.Empty;
        }

        public string GetUserEmail()
        {
            try
            {
                var currentUser = _supabase.Auth.CurrentUser;
                return currentUser?.Email ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetUserEmail failed: {ex.Message}");
                return string.Empty;
            }
        }
        #endregion

        #region Password Management
        public async Task<bool> SendPasswordResetEmailAsync(string email, string? redirectTo = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return false;
                await _supabase.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Password reset failed: {ex.Message}");
                return false;
            }
        }

        public async Task<(UserSession? Session, string? Error)> SignUpWithResultAsync(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignUp(email, password);
                if (session != null && !string.IsNullOrEmpty(session.AccessToken))
                {
                    var jwtToken = new JwtSecurityToken(session.AccessToken);
                    var userSession = new UserSession
                    {
                        AccessToken = session.AccessToken,
                        RefreshToken = session.RefreshToken,
                        ExpiresAt = jwtToken.ValidTo
                    };

                    await SecureStorage.SetAsync("access_token", session.AccessToken);
                    if (!string.IsNullOrEmpty(session.RefreshToken))
                        await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

                    _currentSession = userSession;
                    _isOfflineSessionActive = false;
                    LoggedIn?.Invoke(GetUserId());
                    return (userSession, null);
                }

                return (null, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign up with result failed: {ex.Message}");

                var message = ex.Message ?? string.Empty;
                if (message.Contains("already", StringComparison.OrdinalIgnoreCase) &&
                    message.Contains("register", StringComparison.OrdinalIgnoreCase))
                {
                    return (null, "An account with this email already exists.");
                }
                return (null, "Registration failed. Please try again.");
            }
        }
        #endregion

        #region Events
        public event Action<string>? LoggedIn;
        public event Action? LoggedOut;
        #endregion
    }
}