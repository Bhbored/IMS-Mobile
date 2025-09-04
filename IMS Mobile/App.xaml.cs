using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;
using Supabase.Gotrue;
using Syncfusion.Licensing;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Dispatching;
using static Supabase.Gotrue.Constants;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile
{
    public partial class App : Application
    {
        #region DI
        public static BaseRepository<Transaction>? TransactionRepository { get; set; }
        public static BaseRepository<Product>? ProductRepository { get; set; }
        public static BaseRepository<Contact>? ContactRepository { get; set; }
        public static BaseRepository<TransactionProductItem>? TransactionProductItemRepository { get; set; }
        public static HomeVM? homeVM { get; set; }
        public static ContactsVM? contactsVM { get; set; }
        public static InventoryVM? inventoryVM { get; set; }
        public static ReportsVM? reportsVM { get; set; }

        private readonly Supabase.Client _supabaseClient;
        private readonly SyncService _syncService;
        public static SupabaseAuthService AuthService { get; private set; }
        #endregion


        public App(BaseRepository<Transaction> _transaction, BaseRepository<Product> _productrepo,
            BaseRepository<Contact> _contactrepo, BaseRepository<TransactionProductItem> _transactionProductItemRepo
            , HomeVM _vm, ContactsVM _contactVM, InventoryVM _inventoryVM, ReportsVM _reportsVM,
            Supabase.Client supabaseClient, SyncService syncService, SupabaseAuthService _authservice)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");

            TransactionRepository = _transaction;
            ProductRepository = _productrepo;
            ContactRepository = _contactrepo;
            TransactionProductItemRepository = _transactionProductItemRepo;
            homeVM = _vm;
            contactsVM = _contactVM;
            inventoryVM = _inventoryVM;
            reportsVM = _reportsVM;
            _supabaseClient = supabaseClient;
            _syncService = syncService;
            AuthService = _authservice;
        }

        #region Window Management
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell(AuthService, _syncService);

            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                await HandleInitialNavigation();
            });

            return new Window(shell);
        }

        private async Task HandleInitialNavigation()
        {
            try
            {
                await HandleAuthAndNavigation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App startup error: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                });
            }
        }

        private async Task HandleAuthAndNavigation()
        {
            var accessToken = await SecureStorage.GetAsync("access_token");
            var refreshToken = await SecureStorage.GetAsync("refresh_token");

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var jwtToken = new JwtSecurityToken(accessToken);
                bool isTokenValid = DateTime.UtcNow < jwtToken.ValidTo;

                if (isTokenValid)
                {
                    bool isOnline = NetworkHelper.IsConnected();

                    if (isOnline)
                    {
                        try
                        {
                            await AuthService.InitializeAsync();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"AuthService init failed: {ex.Message}");
                        }
                    }
                    else
                    {
                        AuthService.HydrateOfflineSession(accessToken, refreshToken);

                    }

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    });
                    return;
                }
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            });
        }
        #endregion

        #region Deep Linking
        protected override async void OnAppLinkRequestReceived(Uri uri)
        {
            try
            {
                if (uri?.Scheme?.Equals("imsmobile", StringComparison.OrdinalIgnoreCase) != true ||
                    uri.Host?.Equals("reset-password", StringComparison.OrdinalIgnoreCase) != true)
                {
                    base.OnAppLinkRequestReceived(uri);
                    return;
                }

                var q = ParseQuery(uri.Query);
                var token = q.GetValueOrDefault("token", "");
                var email = q.GetValueOrDefault("email", "");
                var type = q.GetValueOrDefault("type", "");
                string? error = null;

                if (string.Equals(type, "recovery", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(email))
                {
                    try
                    {
                        var auth = ServiceLocator.Services.GetService(typeof(SupabaseAuthService)) as SupabaseAuthService;
                        if (auth is null) error = "Auth service not available.";
                        else
                        {
                            var sb = auth.GetClient();

                            // No IsInitialized property needed — just initialize.
                            await sb.InitializeAsync();

                            var session = await sb.Auth.VerifyOTP(email, token, EmailOtpType.Recovery);

                            if (session != null && !string.IsNullOrEmpty(session.AccessToken) && !string.IsNullOrEmpty(session.RefreshToken))
                                await sb.Auth.SetSession(session.AccessToken, session.RefreshToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }

                var route = $"{nameof(ResetPasswordPage)}" +
                            $"?token={Uri.EscapeDataString(token)}" +
                            $"&email={Uri.EscapeDataString(email)}" +
                            $"&type={Uri.EscapeDataString(type)}";

                if (!string.IsNullOrEmpty(error))
                    route += $"&error={Uri.EscapeDataString(error)}";

                await Shell.Current.GoToAsync(route);
            }
            finally
            {
                base.OnAppLinkRequestReceived(uri);
            }

            static Dictionary<string, string> ParseQuery(string query) =>
                (query ?? string.Empty).TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('=', 2))
                .ToDictionary(
                    a => Uri.UnescapeDataString(a[0]),
                    a => a.Length > 1 ? Uri.UnescapeDataString(a[1]) : string.Empty
                );
        }
        #endregion

        #region Database Management
        public static async Task StopConnection()
        {
            ProductRepository?.WipeAndResetTo1();
            await Task.Delay(100);
            TransactionRepository?.WipeAndResetTo1();
            await Task.Delay(100);
            ContactRepository?.WipeAndResetTo1();
            await Task.Delay(100);
            TransactionProductItemRepository?.WipeAndResetTo1();
        }
        public static async Task RecreateRepositories()
        {
            ProductRepository = new BaseRepository<Product>();
            await Task.Delay(100);
            TransactionRepository = new BaseRepository<Transaction>();
            await Task.Delay(100);
            ContactRepository = new BaseRepository<Contact>();
            await Task.Delay(100);
            TransactionProductItemRepository = new BaseRepository<TransactionProductItem>();
        }
        #endregion
    }
}