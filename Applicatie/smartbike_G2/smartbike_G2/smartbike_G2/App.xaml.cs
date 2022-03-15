using smartbike_G2.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("Nunito-Light.ttf", Alias = "NunitoLight")]
[assembly: ExportFont("Nunito-Regular.ttf", Alias = "NunitoRegular")]
[assembly: ExportFont("Nunito-Medium.ttf", Alias = "NunitoMedium")]
[assembly: ExportFont("Nunito-SemiBold.ttf", Alias = "NunitoSemiBold")]
[assembly: ExportFont("Nunito-Bold.ttf", Alias = "NunitoBold")]

namespace smartbike_G2
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
