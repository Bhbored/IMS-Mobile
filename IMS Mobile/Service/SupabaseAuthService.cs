using Supabase.Gotrue;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Storage;
using IMS_Mobile.MVVM.Models;

namespace IMS_Mobile.Service
{
    public class SupabaseAuthService
    {
        private readonly Supabase.Client _supabase;
        private UserSession? _currentSession;
        private bool _isOfflineSessionActive;

        public SupabaseAuthService(Supabase.Client supabaseClient)
        {
            _supabase = supabaseClient;
        }

        public Supabase.Client GetClient() => _supabase;

        public async Task<bool> InitializeAsync()
        {
            try
            {
                var accessToken = await SecureStorage.GetAsync("access_token");
                var refreshToken = await SecureStorage.GetAsync("refresh_token");

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    await _supabase.Auth.SetSession(accessToken, refreshToken);

                    // Hydrate local session immediately (works offline)
                    var jwtToken = new JwtSecurityToken(accessToken);
                    _currentSession = new UserSession
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresAt = jwtToken.ValidTo
                    };

                    // Try server validation; if offline or server fails, keep offline session
                    var isValid = await ValidateSessionAsync();
                    _isOfflineSessionActive = !isValid;
                    if (isValid)
                    {
                        var userId = GetUserId();
                        LoggedIn?.Invoke(userId);
                    }
                    return true;
                }
                return false;
            }
            catch
            {
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

                    // Store tokens securely
                    await SecureStorage.SetAsync("access_token", session.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", session.RefreshToken);

                    _currentSession = userSession;
                    _isOfflineSessionActive = false;
                    LoggedIn?.Invoke(GetUserId());
                    return userSession;
                }
                return null;
            }
            catch
            {
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
                // Some setups require email confirmation and return no session.
                // Fallback: attempt immediate sign-in to obtain session.
                var signedIn = await SignInAsync(email, password);
                return signedIn;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ValidateSessionAsync()
        {
            try
            {
                // Check if we have a current session
                var accessToken = await SecureStorage.GetAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                    return false;

                // Try to get user with the access token
                var user = await _supabase.Auth.GetUser(accessToken);
                return user != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                await _supabase.Auth.SignOut();
                // Updated: Use Remove instead of RemoveAsync
                SecureStorage.Remove("access_token");
                SecureStorage.Remove("refresh_token");
                _currentSession = null;
                _isOfflineSessionActive = false;
                LoggedOut?.Invoke();
            }
            catch
            {
                // Handle silently
            }
        }

        public bool IsUserAuthenticated => _currentSession != null && !_currentSession.IsExpired;

        public bool IsOfflineSessionActive => _isOfflineSessionActive;

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
                    // Standard claim subject (sub) contains user id in GoTrue
                    var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    return sub ?? string.Empty;
                }
            }
            catch { }
            return string.Empty;
        }

        public event Action<string>? LoggedIn;
        public event Action? LoggedOut;
    }
}