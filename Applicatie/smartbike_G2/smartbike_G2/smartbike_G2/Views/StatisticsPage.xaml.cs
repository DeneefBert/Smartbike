using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using smartbike_G2.Models;
using smartbike_G2.Repositories;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartbike_G2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage()
        {
            InitializeComponent();
            filterIcon.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_filter_green.png");
            pickerStatistics.SelectedItem = "Vandaag";
            TaskExecuter();
        }

        private async void TaskExecuter()
        {
            lblStatus.Text = "Gegevens aan het laden...";
            await ShowTopScoresToday();
        }

        private async Task ShowTopScoresToday()
        {
            List<TopScore> topScoresToday = await SmartbikeRepository.GetTopScoresTodayAsync();
            if (topScoresToday.Count == 0)
            {
                lblStatus.IsVisible = true;
                lblStatus.Text = "Nog geen gegevens beschikbaar.";
            }
            else
            {
                lblStatus.IsVisible = false;
            }
            for (int i = 0; i < topScoresToday.Count; i++)
            {
                topScoresToday[i].Number = i + 1;
            }
            lvwScores.HeightRequest = topScoresToday.Count * 48.5 + 50;
            lvwScores.ItemsSource = topScoresToday;
        }

        private async Task ShowTopScoresThisMonth()
        {
            List<TopScore> topScoresThisMonth = await SmartbikeRepository.GetTopScoresThisMonthAsync();
            if (topScoresThisMonth.Count == 0)
            {
                lblStatus.IsVisible = true;
                lblStatus.Text = "Nog geen gegevens beschikbaar.";
            }
            else
            {
                lblStatus.IsVisible = false;
            }
            for (int i = 0; i < topScoresThisMonth.Count; i++)
            {
                topScoresThisMonth[i].Number = i + 1;
            }

            lvwScores.HeightRequest = topScoresThisMonth.Count * 48.5 + 50;
            lvwScores.ItemsSource = topScoresThisMonth;
        }

        private async Task ShowAllTopScores()
        {
            List<TopScore> topScoresAllTime = await SmartbikeRepository.GetAllTopScoresAsync();
            if (topScoresAllTime.Count == 0)
            {
                lblStatus.IsVisible = true;
                lblStatus.Text = "Nog geen gegevens beschikbaar.";
            }
            else
            {
                lblStatus.IsVisible = false;
            }
            for (int i = 0; i < topScoresAllTime.Count; i++)
            {
                topScoresAllTime[i].Number = i + 1;
            }
            lvwScores.HeightRequest = topScoresAllTime.Count * 48.5 + 50;
            lvwScores.ItemsSource = topScoresAllTime;
        }

        private async void PickerStatistics_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pickerItem = pickerStatistics.SelectedItem.ToString();
            if (pickerItem == "Vandaag")
            {
                await ShowTopScoresToday();
                return;
            }
            if (pickerItem == "Deze maand")
            {
                await ShowTopScoresThisMonth();
                return;
            }
            if (pickerItem == "Altijd")
            {
                await ShowAllTopScores ();
                return;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pickerStatistics.SelectedItem = "Vandaag";
            lblStatus.IsVisible = true;
            TaskExecuter();
        }
    }
}