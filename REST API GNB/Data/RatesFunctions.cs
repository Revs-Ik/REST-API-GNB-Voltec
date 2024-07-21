using REST_API_GNB.Models;
using REST_API_GNB.Connection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace REST_API_GNB.Data
{
    public class RatesFunctions
    {
        // Used in order to establish connection with the DB
        private readonly string _connectionString; 

        public RatesFunctions()
        {
            _connectionString = new ConnectionDB().GetConnectionString();
        }

        public async Task<List<RateModel>> GetAllRates()
        {
            var ratesList = new List<RateModel>();

            // Retrieve all rates from the database using a stored procedure
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var sql = new SqlCommand("showRates", conn))
                {
                    sql.CommandType = CommandType.StoredProcedure;

                    using (var item = await sql.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var rateModel = new RateModel
                            {
                                From = (string)item["from"],
                                To = (string)item["to"],
                                Rate = (decimal)item["rate"]
                            };

                            ratesList.Add(rateModel);
                        }
                    }
                }
            }

            return ratesList;
        }

        public async Task<int> InsertRates(List<RateModel> rates)
        {
            int modifiedRows = 0;

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                foreach (RateModel rate in rates)
                {
                    using (var sql = new SqlCommand("insertRate", conn))
                    {
                        sql.CommandType = CommandType.StoredProcedure;
                        sql.Parameters.AddWithValue("@from", rate.From);
                        sql.Parameters.AddWithValue("@to", rate.To);
                        sql.Parameters.AddWithValue("@rate", rate.Rate);
                        await sql.ExecuteNonQueryAsync();

                        modifiedRows++;
                    }
                }
            }
            return modifiedRows;
        }
    }
}