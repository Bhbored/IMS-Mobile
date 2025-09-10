using IMS_Mobile.MVVM.Models;
using IMS_Mobile.Converters;
using Supabase.Postgrest.Attributes;
using System;

namespace IMS_Mobile.DTOs
{
    [Table("user_preferences")]
    public class UserPreferencesDto : BaseDto
    {
        [Column("theme")]
        public string Theme { get; set; } = "Light";

        [Column("currency")]
        public string Currency { get; set; } = "USD";

        [Column("display_name")]
        public string DisplayName { get; set; } = "";

        [Column("avatar")]
        public string Avatar { get; set; } = "avatar1.png";

        [Column("user_email")]
        public string UserEmail { get; set; } = "";
        public static UserPreferencesDto FromModel(UserPreferences preferences, string currentUserId)
        {
            Guid userId = Guid.Empty;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                Guid.TryParse(currentUserId, out userId);
            }

            return new UserPreferencesDto
            {
                LocalId = preferences.Id,
                UserId = userId,
                Theme = EnumHelper.ThemeToString(preferences.Theme),
                Currency = EnumHelper.CurrencyToString(preferences.Currency),
                DisplayName = preferences.DisplayName,
                Avatar = preferences.Avatar,
                UserEmail = preferences.UserEmail
            };
        }
        public UserPreferences ToModel()
        {
            var currency = EnumHelper.StringToCurrency(this.Currency);
            var theme = EnumHelper.StringToTheme(this.Theme);

            return new UserPreferences
            {
                Id = this.LocalId,
                Theme = theme,
                Currency = currency,
                DisplayName = this.DisplayName,
                Avatar = this.Avatar,
                UserEmail = this.UserEmail
            };
        }
    }
}