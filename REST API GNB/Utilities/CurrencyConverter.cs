using REST_API_GNB.Models;

namespace REST_API_GNB.Utilities
{
    // Código de ChatGPT (con pequeñas modificaciones)
    public class CurrencyConverter
    {
        public async Task<List<TransactionModel>> ConvertToEUR(List<TransactionModel> transactions, List<RateModel> rates)
        {
            var convertedTransactions = new List<TransactionModel>();
            foreach (var transaction in transactions)
            {
                // If transaction already in EUR do not modify
                if (transaction.Currency == "EUR")
                {
                    convertedTransactions.Add(transaction);
                    continue;
                }

                // Otherwise convert it to EUR
                var desiredCurrency = "EUR";
                var conversionPath = FindConversionPath(transaction.Currency, desiredCurrency, rates, out var conversionRate);

                if (conversionPath != null && conversionPath.Count > 0)
                {
                    decimal finalAmount = transaction.Amount;
                    foreach (var rate in conversionPath)
                    {
                        finalAmount *= rate.Rate;
                    }

                    convertedTransactions.Add(new TransactionModel
                    {
                        Sku = transaction.Sku,
                        Amount = finalAmount,
                        Currency = desiredCurrency
                    });
                }
                else
                {
                    StaticLogger.LogLine($"ERROR -> No conversion path found from {transaction.Currency} to {desiredCurrency}", "error.log");
                    throw new Exception($"No conversion path found from {transaction.Currency} to {desiredCurrency}");
                }
            }
            return convertedTransactions;
        }

        private List<RateModel> FindConversionPath(string fromCurrency, string toCurrency, List<RateModel> rates, out decimal totalRate)
        {
            totalRate = 1;
            var visited = new HashSet<string>();
            var path = new List<RateModel>();
            if (DFS(fromCurrency, toCurrency, rates, visited, path))
            {
                totalRate = path.Aggregate(1m, (acc, rate) => acc * rate.Rate);
                return path;
            }
            return null;
        }

        private bool DFS(string current, string target, List<RateModel> rates, HashSet<string> visited, List<RateModel> path)
        {
            if (current == target) return true;
            visited.Add(current);

            foreach (var rate in rates.Where(r => r.From == current && !visited.Contains(r.To)))
            {
                path.Add(rate);
                if (DFS(rate.To, target, rates, visited, path)) return true;
                path.RemoveAt(path.Count - 1); // backtrack
            }
            visited.Remove(current);
            return false;
        }
    }
}