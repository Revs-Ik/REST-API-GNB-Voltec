using Microsoft.AspNetCore.Mvc;
using REST_API_GNB.Data;
using REST_API_GNB.Models;
using REST_API_GNB.Utilities;

namespace REST_API_GNB.Controllers
{
    [ApiController]
    [Route("api/rates")]
    public class RatesController : ControllerBase 
    {
        // SHOW ALL RATES
        [HttpGet("all")]
        public async Task<ActionResult<List<RateModel>>> Get()
        {
            // Log the request
            StaticLogger.LogRequest(HttpContext, "api access.log");

            // Return result
            return Ok(await new RatesFunctions().GetAllRates());
        }

        // ADD NEW RATES
        [HttpPost("add")] 
        public async Task<ActionResult<String>> Post([FromBody] List<RateModel> rates)
        {
            // Log the request
            StaticLogger.LogRequest(HttpContext, "api access.log");

            // Check for api key header
            if (!Request.Headers.TryGetValue("api-key", out var apiKey)) return BadRequest("Missing \"api-key\" header.");

            // Check if authorized api key
            if (!IsValidApiKey(apiKey)) return Unauthorized("Invalid \"api-key\".");

            // Insert rates into to the DB
            var modifiedRows = await new RatesFunctions().InsertRates(rates);
            StaticLogger.LogLine($" --> Added {modifiedRows} rows to the rates table.", "api access.log");

            var resultDict = new { status = 200, modifiedRows = modifiedRows };
            return Ok(resultDict);
        }
        private bool IsValidApiKey(string apiKey)
        {
            return apiKey == "privileged API key";
        }
    }
}
