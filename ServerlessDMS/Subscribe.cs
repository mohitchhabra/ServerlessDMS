using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessDMS.Authentication;
using Microsoft.Extensions.Configuration;
using ServerlessDMS.Services;

namespace ServerlessDMS
{
    public static class Subscribe
    {
        [FunctionName("Subscribe")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                                       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                       .AddEnvironmentVariables()
                                       .Build();

            var authenticationService = new AuthenticationService(config["tenantId"], config["clientId"], config["clientSecret"]);
            var subscriptionService = new SubscriptionService(authenticationService, config["graphEndpoint"], config["notificationEndpoint"]);

            var subscription = await subscriptionService.CreateAsync("created", config["resource"], DateTime.Now.AddMinutes(4230));
            return new OkObjectResult(subscription);
        }
    }
}
