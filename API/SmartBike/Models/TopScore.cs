using System;
using Newtonsoft.Json;

namespace SmartBike.Models
{
    public class TopScore
    {
        public TopScore()
        {
        }
        [JsonProperty("id")]
        public String Id { get; set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty("distance")]
        public Double Distance { get; set; }
        [JsonProperty("calories")]
        public int Calories { get; set; }
        [JsonProperty("score")]
        public int Score { get; set; }
    }
}