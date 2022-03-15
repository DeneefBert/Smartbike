using System;
using Newtonsoft.Json;

namespace smartbike_G2.Models
{
    public class TopScore
    {
        public TopScore()
        {
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty("distance")]
        public double Distance { get; set; }
        [JsonProperty("calories")]
        public int Calories { get; set; }
        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }
        public int Time
        {
            get
            {
                var result = (int)EndTime.Subtract(StartTime).TotalMinutes;
                return result;
            }
        }
    }
}
