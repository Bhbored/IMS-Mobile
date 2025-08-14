using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
  
    public class Product : Entity
    {

        #region fields
        private DateTime createdDate;

        #endregion
     
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Ignore]
        public string FormattedDate => CreatedDate.ToString("dd/MM/yyyy");
        public double Cost { get; set; }
        public int stock { get; set; }

        [Ignore]
        public bool IsChecked { get; set; } = false;
    }

    
    
}
