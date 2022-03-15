using smartbike_G2.Models;
using smartbike_G2.Repositories;
using System;
using Xamarin.Forms;

namespace smartbike_G2.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            AddMenuOptions();
            Loadimage();
        }

        private void AddMenuOptions()
        {
            lvwOptions.ItemsSource = SmartbikeRepository.menuitems;
        }

        private void LvwOptions_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Console.WriteLine(lvwOptions.SelectedItem);
            FlyoutPageItem flyoutItem = lvwOptions.SelectedItem as FlyoutPageItem;

            if (lvwOptions.SelectedItem != null)
            {
                if (flyoutItem.Title == "Speel videos")
                {
                    Shell.Current.GoToAsync("//VideoListPage");
                    lvwOptions.SelectedItem = null;
                    return;
                }

                if (flyoutItem.Title == "Speel muziek")
                {
                    Shell.Current.GoToAsync("//MusicListPage");
                    lvwOptions.SelectedItem = null;
                    return;
                }

                if (flyoutItem.Title == "Speel games")
                {
                    Shell.Current.GoToAsync("//GamePage");
                    lvwOptions.SelectedItem = null;
                    return;
                }
            }
        }

        public void Loadimage()
        {
            Howestlogo.Source = ImageSource.FromResource("smartbike_G2.Resources.Howest_logo_white.png");
        }
        

        // Navigatie naar andere paginas van homescreen

        public void SettingsButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//SettingsPage");
        }
        
    }
}