using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sample.Function
{


    public static class SubmitJob
    {
        public class JobPayload
        {
            public string Id;

            public string[] tasks;

            public string filepath;

            public string version = "v0.0.1";

        }

        [FunctionName("SubmitJob")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "sampleDB",
                collectionName: "sampleContainer",
                ConnectionStringSetting = "CosmosDBConnection")]out JobPayload document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;

            JobPayload jobData = JsonConvert.DeserializeObject<JobPayload>(requestBody);

            jobData.Id = Guid.NewGuid().ToString();

            // send to cosmosDB
            document = jobData;

            return jobData != null
                ? (ActionResult)new OkObjectResult($"Job {jobData.Id} submitted for processing")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
