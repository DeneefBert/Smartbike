using smartbike_G2.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartbike_G2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BikeGamePage : ContentPage
    {
        public List<float> speed = new List<float>();
        public DateTime startTime;
        public DateTime endTime;
        public BikeGamePage()
        {
            InitializeComponent();
            AddBinding();
            startTime = DateTime.Now;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), Main);
        }

        private void AddBinding()
        {
            wvGame.Source = "https://smartbikehowest.github.io/SmartbikeApps/bikegame/frontend/";
        }

        private bool Main()
        {
            SpeedControl();
            return true;
        }
        private async void SpeedControl()
        {
            await wvGame.EvaluateJavaScriptAsync($"setSpeed({SensorRepository.Speed})");
            speed.Add(SensorRepository.Speed);

            //Debug.WriteLine(await wvGame.EvaluateJavaScriptAsync("isFinished()"));

            if ((await wvGame.EvaluateJavaScriptAsync("isLoadStartMenu()")) == "true")
            {
                startMenuScreen.IsVisible = true;
            }

            if ((await wvGame.EvaluateJavaScriptAsync("isFinished()")) == "true")
            {
                string result = await wvGame.EvaluateJavaScriptAsync("getPosition()");
                lblPosition.Text = result;

                float avgSpeed;
                if (speed.Count != 0)
                {
                    avgSpeed = Queryable.Average(speed.AsQueryable());
                }
                else
                {
                    avgSpeed = 0;
                }

                endTime = DateTime.Now;
                int Time = (int)endTime.Subtract(startTime).TotalMinutes;

                lblEndSpeed.Text = $"{Math.Round(avgSpeed,2)} km/u";
                lblEndCalories.Text = $"{Math.Round(3.5 * 62 * 3.5 / 200 * Time, 2)}"; //MET = 3.5

                finishScreen.IsVisible = true;
                await wvGame.EvaluateJavaScriptAsync("setChecked()");
            }
        }

        private void Retry_Clicked(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void ResetGame()
        {
            finishScreen.IsVisible = false;
            wvGame.Reload();
            finishScreen.IsVisible = false;
        }

        private void Stop_Clicked(object sender, EventArgs e)
        {
            finishScreen.IsVisible = false;
            UserRepository.Distance = 0;
            NavigationStuff();
        }

        private async void NavigationStuff()
        {
            await Shell.Current.GoToAsync("//AboutPage");
            Navigation.RemovePage(this);
        }

        private void Easy_Clicked(object sender, EventArgs e)
        {
            startMenuScreen.IsVisible = false;
            StartGame("easy");
        }

        private void Intermediate_Clicked(object sender, EventArgs e)
        {
            startMenuScreen.IsVisible = false;
            StartGame("intermediate");
        }

        private void Hard_Clicked(object sender, EventArgs e)
        {
            startMenuScreen.IsVisible = false;
            StartGame("hard");
        }

        private async void StartGame(string setting)
        {
            await wvGame.EvaluateJavaScriptAsync($"startGame(\"{setting}\")");

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            Navigation.RemovePage(this);
        }
    }
}