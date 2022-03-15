using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Text;
using SmartBike.Models;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Core;

namespace SmartBike
{
    public static class SmartBike
    {
        [FunctionName("AddScore")]
        public static async Task<IActionResult> AddScore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "scores")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                TopScore topScore = JsonConvert.DeserializeObject<TopScore>(json);
                topScore.Id = Guid.NewGuid().ToString();
                topScore.EndTime = DateTime.Now;

                string connectionString = Environment.GetEnvironmentVariable("ConnectionStringCosmosDB");
                CosmosClient cosmosClient = new CosmosClient(connectionString);
                Database database = cosmosClient.GetDatabase("smartbike");
                Container container = database.GetContainer("smartbikecontainer");
                await container.CreateItemAsync<TopScore>(topScore, new PartitionKey(topScore.Id));
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }

            return new OkObjectResult("");
        }

        [FunctionName("GetAllScores")]
        public static async Task<IActionResult> GetAllScores(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scores")] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStringCosmosDB");
            CosmosClient cosmosClient = new CosmosClient(connectionString);
            Database database = cosmosClient.GetDatabase("smartbike");
            Container container = database.GetContainer("smartbikecontainer");

            List<TopScore> topScores = new List<TopScore>();

            QueryDefinition query = new QueryDefinition("SELECT * FROM smartbikecontainer c ORDER BY c.score DESC");

            FeedIterator<TopScore> iterator = container.GetItemQueryIterator<TopScore>(query);
            while (iterator.HasMoreResults)
            {
                FeedResponse<TopScore> response = await iterator.ReadNextAsync();
                topScores.AddRange(response);
            }

            return new OkObjectResult(topScores);
        }

        [FunctionName("GetTop10AllTime")]
        public static async Task<IActionResult> GetTop10AllTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scores/alltime")] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStringCosmosDB");
            CosmosClient cosmosClient = new CosmosClient(connectionString);
            Database database = cosmosClient.GetDatabase("smartbike");
            Container container = database.GetContainer("smartbikecontainer");

            List<TopScore> topScores = new List<TopScore>();

            QueryDefinition query = new QueryDefinition("SELECT * FROM smartbikecontainer c ORDER BY c.score DESC OFFSET 0 LIMIT 10");

            FeedIterator<TopScore> iterator = container.GetItemQueryIterator<TopScore>(query);
            while (iterator.HasMoreResults)
            {
                FeedResponse<TopScore> response = await iterator.ReadNextAsync();
                topScores.AddRange(response);
            }

            return new OkObjectResult(topScores);
        }

        [FunctionName("GetTop10Today")]
        public static async Task<IActionResult> GetTop10Today(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scores/today")] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStringCosmosDB");
            CosmosClient cosmosClient = new CosmosClient(connectionString);
            Database database = cosmosClient.GetDatabase("smartbike");
            Container container = database.GetContainer("smartbikecontainer");

            List<TopScore> topScores = new List<TopScore>();

            QueryDefinition query = new QueryDefinition("SELECT * FROM smartbikecontainer c WHERE SUBSTRING(c.endTime, 1,10) = SUBSTRING(GetCurrentDateTime(),1,10) ORDER BY c.score DESC OFFSET 0 LIMIT 10");

            FeedIterator<TopScore> iterator = container.GetItemQueryIterator<TopScore>(query);
            while (iterator.HasMoreResults)
            {
                FeedResponse<TopScore> response = await iterator.ReadNextAsync();
                topScores.AddRange(response);
            }

            return new OkObjectResult(topScores);
        }

        [FunctionName("GetTop10ThisMonth")]
        public static async Task<IActionResult> GetTop10ThisMonth(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scores/thismonth")] HttpRequest req,
            ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStringCosmosDB");
            CosmosClient cosmosClient = new CosmosClient(connectionString);
            Database database = cosmosClient.GetDatabase("smartbike");
            Container container = database.GetContainer("smartbikecontainer");

            List<TopScore> topScores = new List<TopScore>();

            QueryDefinition query = new QueryDefinition("SELECT * FROM smartbikecontainer c WHERE SUBSTRING(c.endTime, 1,7) = SUBSTRING(GetCurrentDateTime(),1,7) ORDER BY c.score DESC OFFSET 0 LIMIT 10");

            FeedIterator<TopScore> iterator = container.GetItemQueryIterator<TopScore>(query);
            while (iterator.HasMoreResults)
            {
                FeedResponse<TopScore> response = await iterator.ReadNextAsync();
                topScores.AddRange(response);
            }
            return new OkObjectResult(topScores);
        }
    }
}
