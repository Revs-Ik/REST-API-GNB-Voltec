namespace REST_API_GNB.Utilities
{
    public static class StaticLogger
    {
        private static readonly string _logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");
        static StaticLogger()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }
        public static void LogLine(string message, string logFileName)
        {
            var logFilePath = Path.Combine(_logDirectory, logFileName);
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"[{DateTime.Now}]: {message}");
            }
        }
        public static void LogRequest(HttpContext context, string logFileName)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var requestedEndpoint = context.Request.Path + context.Request.QueryString;
            var message = $"Requested {requestedEndpoint} from {ipAddress} ";

            foreach (var header in context.Request.Headers)
            {
                string headerValues = string.Join(", ", header.Value);
                message += ($"\"{header.Key}\": \"{headerValues}\"; ");
            }

            var logFilePath = Path.Combine(_logDirectory, logFileName);
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"[{DateTime.Now}]: {message}");
            }
            
        }
    }
}
