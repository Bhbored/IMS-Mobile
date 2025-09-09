using IMS_Mobile.MVVM.Models;
using SQLite;

namespace IMS_Mobile.DB
{
    public class UserPreferencesRepository : BaseRepository<UserPreferences>
    {
        public UserPreferencesRepository() : base()
        {
            // BaseRepository already creates the table in its constructor
        }

        public UserPreferences GetUserPreferences()
        {
            var preferences = GetItems().FirstOrDefault();

            // If no preferences exist, create default ones
            if (preferences == null)
            {
                preferences = new UserPreferences
                {
                    Theme = Theme.Light,
                    Currency = Currency.USD,
                    DisplayName = "",
                    Avatar = ""
                };
                InsertItem(preferences);
            }

            return preferences;
        }

        public void SaveUserPreferences(UserPreferences preferences)
        {
            var existing = GetItems().FirstOrDefault();

            if (existing != null)
            {
                preferences.Id = existing.Id;
                UpdateItem(preferences);
            }
            else
            {
                InsertItem(preferences);
            }
        }
    }
}
