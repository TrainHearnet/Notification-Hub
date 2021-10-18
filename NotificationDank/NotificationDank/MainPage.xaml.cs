using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NotificationDank
{
    [DesignTimeVisible(false)]

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<string>(this, "Update", (sender) => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Received.Text = sender;
                });
            });
        }
    }
}
