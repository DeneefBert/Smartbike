using smartbike_G2.Repositories;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartbike_G2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        public GamePage()
        {
            InitializeComponent();
            GetData();
        }

        private void GetData()
        {
            lblRule1.Text = "Wanneer je 10 seconden te traag fietst ben je eraan";
            lblRule2.Text = "Als je trager fietst dan de levelsnelheid ga je achteruit";
            lblRule3.Text = "Fiets sneller om sneller vooruit te gaan";

            lblRule4.Text = "Fiets tegen andere fietsers";
            lblRule5.Text = "Finish als eerste om te winnen  ";
            lblRule6.Text = "Fiets zo snel mogeljik om te winnen";
        }

        private void BtnBike_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BikeGamePage());
        }

        private void BtnRun_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RunGamePage());
        }
    }
}