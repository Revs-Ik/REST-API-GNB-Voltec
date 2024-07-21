using REST_API_GNB.Utilities;
using DotNetEnv;

namespace REST_API_GNB.Connection
{
    public class ConnectionDB
    {
        private string _connStringVarName = "CONNECTION_STRING";
        private string? _connectionString = string.Empty;

        public ConnectionDB()
        {
            string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envFilePath))
            {
                Env.Load(envFilePath);
            }
            else
            {
                throw new Exception(".env file not found.");
            }
            
            _connectionString = Environment.GetEnvironmentVariable(_connStringVarName);
            if (_connectionString == string.Empty || _connectionString == null)
            {
                throw new Exception($"Connection string var {{{_connStringVarName}}} not found.");
            }
        }
        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
