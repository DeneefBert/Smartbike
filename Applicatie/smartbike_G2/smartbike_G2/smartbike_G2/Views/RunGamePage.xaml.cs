using smartbike_G2.Repositories;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartbike_G2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunGamePage : ContentPage
    {
        private bool IsRunning = false;
        private int TimeSpent = 0;
        private int Level;
        public List<float> speed = new List<float>();
        public DateTime startTime;
        public DateTime endTime;
        public RunGamePage()
        {
            InitializeComponent();
            startTime = DateTime.Now;
        }

        private void AddBinding()
        {
            wvGame.Source = "https://smartbikehowest.github.io/SmartbikeApps/rungame/";
        }

        private bool CheckPlayer()
        {
            SpeedControl();
            return IsRunning;
        }

        private async void SpeedControl()
        {
            string result = await wvGame.EvaluateJavaScriptAsync($"getSpeed({SensorRepository.Speed})");
            if (result != "on")
            {
                Level = int.Parse(result);
                IsRunning = false;
                FillEndScreen();
            }
        }

        private void FillEndScreen()
        {
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

            lblLevel.Text = $"Je behaalde level {Level}";
            lblEndSpeed.Text = $"{Math.Round(avgSpeed, 2)} km/u";
            lblEndCalories.Text = $"{Math.Round(3.5 * 62 * 3.5 / 200 * Time, 2)}"; //MET = 3.5

            finishScreen.IsVisible = true;
        }

        private async void StartGame()
        {
            await wvGame.EvaluateJavaScriptAsync($"startGame()");
            IsRunning = true;
            Device.StartTimer(new TimeSpan(0, 0, 1), TimerFunction);
            // Timer function, checks every 200 miliseconds whether the user is fast enough
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), CheckPlayer);
        }

        private bool TimerFunction()
        {
            TimeSpent++;
            return IsRunning;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            AddBinding();
            StartButton.IsVisible = true;

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            IsRunning = false;
            Navigation.RemovePage(this);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            StartGame();
            StartButton.IsVisible = false;
        }

        private void Retry_Clicked(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void ResetGame()
        {
            finishScreen.IsVisible = false;
            UserRepository.Distance = 0;
            AddBinding();
            StartButton.IsVisible = true;
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
        }
    }
}