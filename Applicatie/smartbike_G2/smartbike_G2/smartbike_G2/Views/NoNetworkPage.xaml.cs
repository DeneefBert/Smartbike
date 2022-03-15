using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace smartbike_G2.Views
{
    public partial class NoNetworkPage : ContentPage
    {
        public NoNetworkPage()
        {
            InitializeComponent();
            Connectivity.ConnectivityChanged += ConnectivityChangedHandler; //When connectivity changes again
        }

        private void ConnectivityChangedHandler(object sender, ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet) //Internet available
            {
                Shell.Current.GoToAsync("//AboutPage"); //Go back to main page
            }
        }
    }
}
