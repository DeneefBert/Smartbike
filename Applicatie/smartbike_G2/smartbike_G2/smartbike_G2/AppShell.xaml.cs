using smartbike_G2.Repositories;
using smartbike_G2.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace smartbike_G2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            SmartbikeRepository.ResetFlyoutItems();
            SmartbikeRepository.ResetMenuItems();
            Routing.RegisterRoute(nameof(VideoListPage), typeof(VideoListPage));
            Routing.RegisterRoute(nameof(MusicListPage), typeof(MusicListPage));
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
            Routing.RegisterRoute(nameof(StatisticsPage), typeof(StatisticsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            // later usage, not in flyout or main menu
            //Routing.RegisterRoute(nameof(VideoPage), typeof(VideoPage));
            SensorRepository blt = new SensorRepository();
            blt.Init();
            //blt.scanDevices();
            blt.ConnectDevice();

            //Check connectivity when app is opened
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                Navigation.PushAsync(new NoNetworkPage());
            }
            Connectivity.ConnectivityChanged += ConnectivityChangedHandler; //When connectivity changes
        }

        private void ConnectivityChangedHandler(object sender, ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                Navigation.PushAsync(new NoNetworkPage());
            }
        }

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Shell.Current.GoToAsync("//LoginPage");
        //}
        protected override bool OnBackButtonPressed()
        {
            Console.WriteLine("Backbutton denied");
            Current.FlyoutIsPresented = !Current.FlyoutIsPresented;
            return true;
        }
    }
}
