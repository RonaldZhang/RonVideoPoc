using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RonVideo.Models;
using System.Diagnostics.CodeAnalysis;

namespace RonVideo
{
    [ExcludeFromCodeCoverage]
    public static class BlendLoanIdSimulator
    {
        [FunctionName("BlendLoanIdSimulator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = Constants.RouteLoanId)] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get LoanId Called BlendId: {id}");

            string name = req.Query["name"];

            BlendLoanIdResponse resp = new BlendLoanIdResponse();
            resp.Id = id;  
            resp.LoanId = Guid.NewGuid().ToString();
            await Task.Delay(500);
            return new OkObjectResult(resp);
        }
    }
}
