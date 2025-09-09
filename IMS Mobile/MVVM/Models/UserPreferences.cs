using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    public enum Theme
    {
        Light,
        Dark
    }

    public enum Currency
    {
        USD,
        EUR,
        GBP,
        JPY,
        LBP
    }

    public class UserPreferences : Entity
    {
        public Theme Theme { get; set; } = Theme.Light;
        public Currency Currency { get; set; } = Currency.USD;
        public string DisplayName { get; set; } = "";
        public string Avatar { get; set; } = "";
    }
}
