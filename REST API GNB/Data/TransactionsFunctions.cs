using REST_API_GNB.Models;
using REST_API_GNB.Connection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace REST_API_GNB.Data
{
    public class TransactionsFunctions
    {
        // Used in order to establish connection with the DB
        private readonly string _connectionString;
        public TransactionsFunctions()
        {
            _connectionString = new ConnectionDB().GetConnectionString();
        }

        public async Task<List<TransactionModel>> GetAllTransactions()
        {
            var transactionsList = new List<TransactionModel>();

            // Retrieve all transactions from the database using a stored procedure
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var sql = new SqlCommand("showTransactions", conn))
                {
                    sql.CommandType = CommandType.StoredProcedure;

                    using (var item = await sql.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var transactionModel = new TransactionModel
                            {
                                Sku = (string)item["sku"],
                                Amount = (decimal)item["amount"],
                                Currency = (string)item["currency"]
                            };

                            transactionsList.Add(transactionModel);
                        }
                    }
                }
            }
            return transactionsList;
        }

        public async Task<int> InsertTransactions(List<TransactionModel> transactions)
        {
            int modifiedRows = 0;

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                foreach (TransactionModel transaction in transactions)
                {
                    using (var sql = new SqlCommand("insertTransaction", conn))
                    {
                        sql.CommandType = CommandType.StoredProcedure;
                        sql.Parameters.AddWithValue("@sku", transaction.Sku);
                        sql.Parameters.AddWithValue("@amount", transaction.Amount);
                        sql.Parameters.AddWithValue("@currency", transaction.Currency);
                        await sql.ExecuteNonQueryAsync();

                        modifiedRows++;
                    }
                }
            }
            return modifiedRows;
        }

        public async Task<List<TransactionModel>> SearchTransactions(string sku)
        {
            var matchingTransactions = new List<TransactionModel>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var sql = new SqlCommand($"SELECT * FROM transactions WHERE sku = @sku", conn))
                {
                    sql.Parameters.AddWithValue("@sku", sku);
                    using (var item = await sql.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var transactionModel = new TransactionModel
                            {
                                Sku = (string)item["sku"],
                                Amount = (decimal)item["amount"],
                                Currency = (string)item["currency"]
                            };

                            matchingTransactions.Add(transactionModel);
                        }
                    }
                }
            }

            return matchingTransactions;
        }
    }
}