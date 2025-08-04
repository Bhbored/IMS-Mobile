using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = IMS_Mobile.MVVM.Models.Product;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class InventoryVM
    {

        #region fields
        #endregion

        #region Properties
        public ObservableCollection<Product> Products { get; set; } = [new Product
            {
                Id = 1,
                Name = "Organic Apples",
                Price = 2.50,
                Cost = 1.00,
                stock = 10,
                CreatedDate = DateTime.Now
            },

            new Product
            {
                Id = 2,
                Name = "Fresh Bananas",
                Price = 1.20,
                Cost = 0.50,
                stock = 5,
                CreatedDate = DateTime.Now
            },

            new Product
            {
                Id = 3,
                Name = "Ripe Oranges",
                Price = 1.80,
                Cost = 0.75,
                stock = 15,
                CreatedDate = DateTime.Now
            },

            new Product
            {
                Id = 4,
                Name = "Sweet Strawberries",
                Price = 3.00,
                Cost = 1.50,
                stock = 8,
                CreatedDate = DateTime.Now
            },

            new Product
            {
                Id = 5,
                Name = "Green Grapes",
                Price = 2.00,
                Cost = 0.90,
                stock = 12,
                CreatedDate = DateTime.Now
            }];
        #endregion

        #region Methods

        #endregion

        #region commands
        #endregion

        #region Tasks
        #endregion
    }
}