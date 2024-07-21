using Microsoft.AspNetCore.Mvc;
using REST_API_GNB.Data;
using REST_API_GNB.Models;
using REST_API_GNB.Utilities;

namespace REST_API_GNB.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        // SHOW ALL TRANSACTIONS
        [HttpGet("all")]
        public async Task<ActionResult<List<TransactionModel>>> Get()
        {
            // Log the request
            StaticLogger.LogRequest(HttpContext, "api access.log");

            // Return result
            return Ok(await new TransactionsFunctions().GetAllTransactions());
        }

        // ADD NEW TRANSACTIONS
        [HttpPost("add")] 
        public async Task<ActionResult<String>> Post([FromBody] List<TransactionModel> transactions)
        {
            // Log the request
            StaticLogger.LogRequest(HttpContext, "api access.log");

            // Check for api key header
            if (!Request.Headers.TryGetValue("api-key", out var apiKey)) return BadRequest("Missing \"api-key\" header.");

            // Check if authorized api key
            if (!IsValidApiKey(apiKey)) return Unauthorized("Invalid \"api-key\".");

            // Insert transactions to the DB
            var modifiedRows = await new TransactionsFunctions().InsertTransactions(transactions);
            StaticLogger.LogLine($" --> Added {modifiedRows} rows to the transactions table.", "api access.log");

            var resultDict = new { status = 200, modifiedRows = modifiedRows };
            return Ok(resultDict);
        }

        // SEARCH TRANSACTIONS BY SKU (converted into EUR)
        [HttpGet("search")]
        public async Task<ActionResult<String>> Search([FromQuery] string? sku) // (accept null to modify the error message)
        {
            // Log the request
            StaticLogger.LogRequest(HttpContext, "api access.log");

            // Check that the request has a query sku parameter
            if (string.IsNullOrEmpty(sku)) return BadRequest("Invalid query: \"sku\" parameter is required.");

            // Find all matching SKU transactions
            var matchingTransactions = await new TransactionsFunctions().SearchTransactions(sku);

            // Return 404 if no matching transactions exist
            if (matchingTransactions.Count == 0) return NotFound("No transactions found for the resquested SKU");

            // Convert transaction currency to EUR
            var rates = await new RatesFunctions().GetAllRates();
            var convertedTransactions = await new CurrencyConverter().ConvertToEUR(matchingTransactions, rates);

            // Calculate the total sum of all transactions
            var totalAmount = new decimal();
            foreach (var transaction in convertedTransactions) totalAmount += transaction.Amount;
            var resultDict = new {totalAmount = totalAmount, transactions = convertedTransactions};

            return Ok(resultDict);
        }
    
        private bool IsValidApiKey(string apiKey)
        {
            return apiKey == "privileged API key"; // We could store api keys in the database and check them here.
        }

    }
}
