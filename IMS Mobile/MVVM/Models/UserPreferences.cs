using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    public class UserPreferences : Entity
    {
        public string Theme { get; set; } = "Light";
        public string Currency { get; set; } = "USD";
        public string DisplayName { get; set; } = "";
        public string Avatar { get; set; } = "";
    }
}
