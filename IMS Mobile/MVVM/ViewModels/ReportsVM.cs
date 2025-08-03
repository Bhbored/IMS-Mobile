using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cashRegister = IMS_Mobile.MVVM.Models.cashRegister;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ReportsVM
    {

        #region fields
        #endregion

        #region Properties
        public cashRegister CashRegisterData { get; set; } = new cashRegister
        {
            CashFlow = 12500.75,
            InventoryValue = 8500.50,
            TotalSales = 25000.00,
            TotalCredit = 3500.25
        };
        #endregion

        #region Methods

        #endregion

        #region commands
        #endregion

        #region Tasks
        #endregion
    }
}