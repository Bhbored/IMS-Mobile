using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using IMS_Mobile.MVVM.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.Service
{

    public class ModernToastService
    {
        public static InventoryPage? InventoryPageInstance { get; set; }
        public static InventoryPage InventoryPage
        {
            get => InventoryPageInstance;
            set
            {
                InventoryPageInstance = value;
            }
        }

        public static async Task ShowError(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                var snackbar = Snackbar.Make(
                message: message,
                duration: TimeSpan.FromSeconds(2),
                visualOptions: new SnackbarOptions
                {
                    BackgroundColor = Colors.Red,
                    TextColor = Colors.White,
                    CornerRadius = 10,

                },
                anchor: InventoryPageInstance
            );
                await snackbar.Show();
            });
        }

        public static async Task ShowSuccess(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                var snackbar = Snackbar.Make(
                message: message,
                duration: TimeSpan.FromSeconds(2),
                visualOptions: new SnackbarOptions
                {
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White,
                    CornerRadius = 10,

                },
                anchor: InventoryPageInstance
            );
                await snackbar.Show();
            });
        }

        public static async Task ShowInfo(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                var snackbar = Snackbar.Make(
                message: message,
                duration: TimeSpan.FromSeconds(2),
                visualOptions: new SnackbarOptions
                {
                    BackgroundColor = Colors.Blue,
                    TextColor = Colors.White,
                    CornerRadius = 10,

                },
                anchor: InventoryPageInstance
            );
                await snackbar.Show();
            });
        }


    }
}

