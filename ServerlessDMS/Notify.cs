using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessDMS.Services;
using ServerlessDMS.Authentication;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ServerlessDMS
{
    public static class Notify
    {
        [FunctionName("Notify")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notify")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                                       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                       .AddEnvironmentVariables()
                                       .Build();

            // Case Webhook is validating endpoint for new subscription
            string validationToken = req.Query["validationToken"];
            if (validationToken != null)
            {
                log.LogInformation($"Validating Token ({validationToken}) to create Subscription...");
                return new OkObjectResult(validationToken);
            }

            // Case notification sent from Webhook
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string resource = data?.value?[0]?.resource;
            string changeType = data?.value?[0]?.changeType;

            log.LogInformation($"Processing incoming notification from Microsoft Graph...");
            log.LogInformation($"ChangeType: {changeType}");
            log.LogInformation($"Resource: {resource}");

            // Abort notification processing if the notification is not about a newly created (=arrived) message. This is necessary because the Microsoft Graph calls multiple times, when a messages arrives with changeType created and updated
            if (changeType != "created")
            {
                return new StatusCodeResult(202);
            }

            // Abost if no resource to fetch an attachment from is provided
            if (string.IsNullOrEmpty(resource))
            {
                return new BadRequestObjectResult("Invalid notification format. See here for expected format: https://docs.microsoft.com/en-us/graph/webhooks#managing-subscriptions");
            }

            var authenticationService = new AuthenticationService(config["tenantId"], config["clientId"], config["clientSecret"]);
            var mailService = new MailService(authenticationService, config["graphEndpoint"]);

            // Get First Attachment of mail
            var attachments = await mailService.GetAttachmentByMailResourceAsync(resource);
            var firstAttachment = attachments.First();

            // Upload attachment to Sharepoint
            var fileService = new FileService(authenticationService, config["graphEndpoint"]);
            var path = $"{config["graphEndpoint"]}sites/{config["siteId"]}/drive/items/";
            var driveItem = await fileService.UploadFileAsync($"{path}root:/{firstAttachment.Name}:/content", firstAttachment.ContentType, Convert.FromBase64String(firstAttachment.ContentBytes));

            // Convert attachment to PDF
            var pdfBytes = await fileService.ConvertFileAsync(path, driveItem.Id, "pdf");
            var pdfName = $"{firstAttachment.Name}.pdf";

            // Upload pdf version of attachment to Sharepoint
            var pdfItem = await fileService.UploadFileAsync($"{path}root:/{pdfName}:/content", "application/pdf", pdfBytes);

            log.LogInformation($"Processed notification");
            return new StatusCodeResult(202);
        }
    }
}
