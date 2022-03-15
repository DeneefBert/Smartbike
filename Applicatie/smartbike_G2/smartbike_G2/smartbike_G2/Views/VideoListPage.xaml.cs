using smartbike_G2.Administration;
using smartbike_G2.Models;
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
    public partial class VideoListPage : ContentPage
    {
        private string MyVideo { get; set; }
        private string MyVideoName { get; set; }
        public bool IsFast = false;
        public float MinimumSpeed = SetupData.MinimumSpeed;
        private List<VideoData> Playlist { get; set; }
        private List<PlaylistData> Videos = new List<PlaylistData>();
        public bool IsFullScreen = false;
        private bool PageOpen = false;

        private bool TurnedOn = false;
        private bool KeepRunning = true;
        private int TimeSpent = 0;

        public List<float> speed = new List<float>();
        public DateTime startTime;
        public DateTime endTime;
        private int NotBiking = 0;

        public VideoListPage()
        {
            InitializeComponent();

            // Load in genre picker
            GetPlaylists();

            // Binding images to the buttons
            imgFullscreen.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_fullscreen.png");
            filterIcon.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_filter_green.png");
        }

        // Bool function to call the main speed control function. Returns true to restart timer
        private bool Main()
        {
            UpdateDistance();
            CheckPlayer();
            return PageOpen;
        }

        private void UpdateDistance()
        {
            string distanceTxt;
            if (UserRepository.Distance < 1000)
            {
                distanceTxt = $"Afstand: {Math.Round(UserRepository.Distance)} m";
            }
            else
            {
                distanceTxt = $"Afstand: {Math.Round(UserRepository.Distance / 1000.0),2} km";
            }
            lblData.Text = $"Snelheid: {Math.Round(SensorRepository.Speed, 1)} km/u\n{distanceTxt}";
            if (SensorRepository.Speed != 0.0)
            {
                speed.Add(SensorRepository.Speed);
            }
        }

        private void CheckPlayer()
        {
            PlayerControl();
        }

        // Main video selection function. IsSet on true if you want to play a specific video, specified in MyVideo
        // If IsSet is false, this function will pick a random video from the playlist by shuffling the playlist
        // In both cases, the rest of the playlist gets added to the play queue for a smoother experience
        private async void GetVideos(bool IsSet)
        {
            Playlist = await SmartbikeRepository.GetVideosDataAsync();
            Debug.WriteLine("video data received");
            Random rnd = new Random();
            int n = Playlist.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                VideoData value = Playlist[k];
                Playlist[k] = Playlist[n];
                Playlist[n] = value;
            }
            string url = $"{Playlist[0].VideoId}";
            foreach(VideoData v in Playlist.GetRange(1, Playlist.Count - 1))
            {
                url += $",{v.VideoId}";
            }
            if (IsSet)
            {
                mmVideo.Source = $"https://smartbikehowest.github.io/SmartbikeApps/player/?id={MyVideo}&playlistid={url}";
                lblTitle.Text = MyVideoName;
            }
            else
            {
                mmVideo.Source = $"https://smartbikehowest.github.io/SmartbikeApps/player/?id={Playlist[0].VideoId}&playlistid={url}";
                lblTitle.Text = Playlist[0].Title;
            }
            lvwVideos.ItemsSource = Playlist.GetRange(1, Playlist.Count - 1);
        }

        // Function which fetches all the playlists of the connected youtube channel
        // These then get added to the category picker
        // The picker is disabled during this time to prevent fast users from crashing the application
        private async void GetPlaylists()
        {
            pickerGenre.IsEnabled = false;
            Videos = new List<PlaylistData>();
            List<PlaylistData> Channel = await SmartbikeRepository.GetPlaylistsDataAsync(SetupData.ChannelID);
            foreach (var item in Channel)
            {
                // Check if the playlist is a music or video playlist
                // If yes add to Video list and add title minus the "Videos - " part to the picker
                if (item.Title.ToLower().Contains("videos"))
                {
                    Videos.Add(item);
                    pickerGenre.Items.Add(item.Title.Substring(9));
                }
            }
            pickerGenre.IsEnabled = true;
        }

        // Function to change the current video to the one selected in the sidebar
        private void LvwVideos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MyVideo = (lvwVideos.SelectedItem as VideoData).VideoId;
            MyVideoName = (lvwVideos.SelectedItem as VideoData).Title;
            GetVideos(true);
            InfoStartVideo.IsVisible = true;
        }

        // Main video control function
        // Compares current speed to minimum speed and adjusts the player accordingly
        private async void PlayerControl()
        {
            IsFast = MinimumSpeed < SensorRepository.Speed;
            string state = await mmVideo.EvaluateJavaScriptAsync("player.getPlayerState()");
            if (!TurnedOn && ((state == "1") || (state == "2")))
            {
                TurnedOn = true;
                InfoStartVideo.IsVisible = false;
                lblMinSpeed.Text = $"Min. snelheid:\n{MinimumSpeed} km/u";
                Device.StartTimer(new TimeSpan(0, 0, 1), TimerFunction);
            }
            else
            {
                if (!IsFast && (state == "1"))
                {
                    await mmVideo.EvaluateJavaScriptAsync("player.pauseVideo()");

                }
                else if (IsFast && (state == "2"))
                {
                    await mmVideo.EvaluateJavaScriptAsync("player.playVideo()");
                }
            }            
        }

        // Timer function for start game
        private bool TimerFunction()
        {
            TimeSpent++;
            if (!IsFast)
            {
                NotBiking++;
                if (NotBiking == 30)
                {
                    SummonMenu();
                }
            } else
            {
                NotBiking = 0;
            }
            if ((TimeSpent % SetupData.TimeInterval == 0) && (MinimumSpeed < SetupData.MaxSpeed))
            {
                MinimumSpeed += SetupData.SpeedInterval;
                lblMinSpeed.Text = $"Min. snelheid:\n{MinimumSpeed} km/u";
            }
            return KeepRunning;
        }

        // End screen button
        private void EndSessionButton_Clicked(object sender, EventArgs e)
        {
            SummonMenu();
        }

        private void SummonMenu()
        {
            EndSessionScreen.IsVisible = true;
            KeepRunning = false;

            endTime = DateTime.Now;
            double distance = UserRepository.Distance / 1000.0;
            lblEndDistance.Text = Math.Round(distance, 2).ToString();
            float avgSpeed;
            if (speed.Count != 0)
            {
                avgSpeed = Queryable.Average(speed.AsQueryable());
            }
            else
            {
                avgSpeed = 0;
            }
            lblEndSpeed.Text = Math.Round(avgSpeed, 2).ToString();
            //int time = (int)endTime.Subtract(startTime).TotalMinutes;
            //int calories = (int)(3.5 * 62 * 3.5 / 200 * time / 60); //MET = 3.5
            int calories = (int)(3.5 * 62 * 3.5 / 200 * TimeSpent / 60); //MET = 3.5
            lblEndCalories.Text = calories.ToString();
            int score = (int)(distance * avgSpeed);
            lblEndScore.Text = score.ToString();

        }

        // Menu navigation
        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            EndSessionScreen.IsVisible = false;
            ExitFullScreen();
            UserRepository.Distance = 0;
            Shell.Current.GoToAsync("//AboutPage");
        }

        // Save score
        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            TopScore score = new TopScore
            {
                Name = EntryUsername.Text,
                Distance = UserRepository.Distance / 1000.0
            };
            float avgSpeed;
            if (speed.Count != 0)
            {
                avgSpeed = Queryable.Average(speed.AsQueryable());
            }
            else
            {
                avgSpeed = 0;
            }
            score.StartTime = startTime;
            score.EndTime = endTime;
            score.Calories = (int)(3.5 * 62 * 3.5 / 200 * score.Time); //MET = 3.5
            score.Score = (int)(score.Distance * avgSpeed);

            await SmartbikeRepository.AddTopScoreAsync(score);

            EndSessionScreen.IsVisible = false;
            ExitFullScreen();

            // Reset everything to default, functions are async so they will continue while the menu page is on

            await Shell.Current.GoToAsync("//StatisticsPage");
        }

        // Function to make the player fullscreen
        private void OpenFullScreen()
        {
            lblTitle.IsVisible = false;
            videoOptions.IsVisible = false;
            Grid.SetColumn(leftSideGrid, 0);
            Grid.SetColumnSpan(leftSideGrid, 5);

            Grid.SetRow(mmVideo, 0);
            Grid.SetRowSpan(mmVideo, 6);
            Grid.SetRow(PlayerBar, 5);

            IsFullScreen = true;
        }

        // Function to make the player smaller again
        private void ExitFullScreen()
        {
            lblTitle.IsVisible = true;
            videoOptions.IsVisible = true;
            Grid.SetColumn(leftSideGrid, 1);
            Grid.SetColumnSpan(leftSideGrid, 1);

            Grid.SetRow(mmVideo, 1);
            Grid.SetRowSpan(mmVideo, 1);
            Grid.SetRow(PlayerBar, 1);
            Grid.SetRowSpan(PlayerBar, 1);
            PlayerBar.HeightRequest = 40;

            IsFullScreen = false;
        }

        // Function to change the playlist when the user selects another category
        private void PickerGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            SmartbikeRepository.PlaylistID = Videos[pickerGenre.SelectedIndex].PlaylistId;
            GetVideos(false);
        }

        // Button function for the fullscreen button
        private void ImgFullscreen_Clicked(object sender, EventArgs e)
        {
            if (!IsFullScreen)
            {
                OpenFullScreen();
            }
            else
            {
                ExitFullScreen();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SmartbikeRepository.PlaylistID = SetupData.DefaultVideos;
            GetVideos(false);
            PageOpen = true;
            TurnedOn = false;
            KeepRunning = true;
            TimeSpent = 0;
            InfoStartVideo.IsVisible = true;
            EntryUsername.Text = "";
            startTime = DateTime.Now;
            // Timer function, checks every 200 miliseconds whether the user is fast enough
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), Main);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MinimumSpeed = 10;
            KeepRunning = false;
            PageOpen = false;
        }
    }
}