using PropertyChanged;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Product : Entity
    {

        #region fields
        private DateTime createdDate;
        private int _quantity = 1;
        #endregion

        [Ignore]
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value && value > 0)
                {
                    _quantity = value;
                    
                }
            }
        }
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
