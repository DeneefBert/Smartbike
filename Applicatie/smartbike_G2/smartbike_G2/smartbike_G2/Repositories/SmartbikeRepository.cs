using Newtonsoft.Json;
using smartbike_G2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;


namespace smartbike_G2.Repositories
{
    class SmartbikeRepository
    {
        public static List<FlyoutPageItem> flyoutitems = new List<FlyoutPageItem>() {};
        public static List<FlyoutPageItem> menuitems = new List<FlyoutPageItem>() { };

        public static string PlaylistID = "PLnGa7tcaV-U_fyGokchMfwy5O2efrmz5a";

        public static void ResetFlyoutItems()
        {
            List<FlyoutPageItem> tempflyoutitems = new List<FlyoutPageItem>() { };
            FlyoutPageItem Home = new FlyoutPageItem("Home", "AboutPage", "icon_home");
            FlyoutPageItem Videos = new FlyoutPageItem("Videos", "VideoListPage", "icon_videos");
            FlyoutPageItem Music = new FlyoutPageItem("Music", "MusicListPage", "icon_music");
            FlyoutPageItem Game = new FlyoutPageItem("Game", "GamePage", "icon_game");
            FlyoutPageItem Statistics = new FlyoutPageItem("Statistics", "StatisticsPage", "icon_statistics");
            FlyoutPageItem Settings = new FlyoutPageItem("Settings", "SettingsPage", "icon_settings");

            tempflyoutitems.Add(Home);
            tempflyoutitems.Add(Videos);
            tempflyoutitems.Add(Music);
            tempflyoutitems.Add(Game);
            tempflyoutitems.Add(Statistics);
            tempflyoutitems.Add(Settings);

            flyoutitems = tempflyoutitems;

        }
        public static void ResetMenuItems()
        {
            List<FlyoutPageItem> tempmenuitems = new List<FlyoutPageItem>() { };
            FlyoutPageItem Videos = new FlyoutPageItem("Speel videos", "VideoListPage", "icon_videos");
            FlyoutPageItem Music = new FlyoutPageItem("Speel muziek", "MusicListPage", "icon_music");
            FlyoutPageItem Game = new FlyoutPageItem("Speel games", "AboutPage", "icon_game");

            tempmenuitems.Add(Videos);
            tempmenuitems.Add(Music);
            tempmenuitems.Add(Game);

            menuitems = tempmenuitems;
        }

        private const string _COSMOSURL = "https://smartbikefunctionapp.azurewebsites.net/api/";
        private const string _VIDEOURL = "https://youtube.googleapis.com/youtube/v3";
        private const string _APIkey = "AIzaSyAhxt_uyk9H9O9epUHmxnAhpTd5n57doAE";
        private static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            return client;
        }

        public static async Task<List<TopScore>> AddTopScoreAsync(TopScore topScore)
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string json = JsonConvert.SerializeObject(topScore);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{_COSMOSURL}scores", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Something went wrong: {response.StatusCode}");
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static async Task<List<TopScore>> GetTopScoresTodayAsync()
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string jsonString = await client.GetStringAsync($"{_COSMOSURL}scores/today");
                    if (jsonString != null)
                    {
                        return JsonConvert.DeserializeObject<List<TopScore>>(jsonString);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error receiving Json data in GetTopScoresTodayAsyncc()");
                    Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        public static async Task<List<TopScore>> GetTopScoresThisMonthAsync()
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string jsonString = await client.GetStringAsync($"{_COSMOSURL}scores/thismonth");
                    if (jsonString != null)
                    {
                        return JsonConvert.DeserializeObject<List<TopScore>>(jsonString);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error receiving Json data in GetTopScoresThisMonthAsync()");
                    Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        public static async Task<List<TopScore>> GetAllTopScoresAsync()
        {
            using (HttpClient client = GetClient())
            {
                try
                {
                    string jsonString = await client.GetStringAsync($"{_COSMOSURL}scores/alltime");
                    if (jsonString != null)
                    {
                        return JsonConvert.DeserializeObject<List<TopScore>>(jsonString);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error receiving Json data in GetAllTopScoresAsync()");
                    Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        public static async Task<List<VideoData>> GetVideosDataAsync()
        {
            try
            {
                using (HttpClient client = GetClient())
                {
                    string url = $"{_VIDEOURL}/playlistItems?part=snippet&maxResults=50&playlistId={PlaylistID}&key={_APIkey}";
                    Debug.WriteLine(url);
                    string jsonString = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(jsonString);
                    if (json != null)
                    {
                        JToken items = json.SelectToken("items");
                        List<VideoData> videodatalist = new List<VideoData>();

                        foreach (var item in items)
                        {
                            JToken snippet = item.SelectToken("snippet");
                            VideoData videodata = new VideoData();
                            JToken thumbnail = snippet.SelectToken("thumbnails").SelectToken("medium").SelectToken("url"); // Could go on higher but not every video added will have this
                            JToken title = snippet.SelectToken("title");
                            JToken videoId = snippet.SelectToken("resourceId").SelectToken("videoId");
                            videodata.ThumbnailUrl = thumbnail.ToString();
                            videodata.Title = title.ToString();
                            videodata.VideoId = videoId.ToString();

                            videodatalist.Add(videodata);
                        }
                        Debug.WriteLine("received Json video data");
                        
                        return videodatalist;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error receiving Json data in GetVideosDataAsync()");
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static async Task<List<PlaylistData>> GetPlaylistsDataAsync(string channelId)
        {
            try
            {
                using (HttpClient client = GetClient())
                {
                    string url = $"{_VIDEOURL}/playlists?part=snippet&maxResults=50&channelId={channelId}&key={_APIkey}";
                    Debug.WriteLine(url);
                    string jsonString = await client.GetStringAsync(url);
                    JObject json = JObject.Parse(jsonString);

                    if (json != null)
                    {
                        JToken items = json.SelectToken("items");
                        List<PlaylistData> playlistdatalist = new List<PlaylistData>();

                        foreach (var item in items)
                        {
                            JToken snippet = item.SelectToken("snippet");
                            PlaylistData playlistdata = new PlaylistData();
                            JToken title = snippet.SelectToken("title");
                            JToken playlistId = item.SelectToken("id");

                            playlistdata.Title = title.ToString();
                            playlistdata.PlaylistId = playlistId.ToString();
                            playlistdatalist.Add(playlistdata);
                        }
                        Debug.WriteLine("received Json video data");

                        return playlistdatalist;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error receiving Json data in GetPlaylistsDataAsync()");
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}