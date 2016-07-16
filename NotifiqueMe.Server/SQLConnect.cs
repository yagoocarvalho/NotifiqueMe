using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;

namespace NotifiqueMe.Server
{
    class SQLConnect
    {
        private const string DATASOURCE = "localhost";
        private const string DBNAME = "GCAPPDB";

        Queue<LogEntry> logQueue;
        public bool verbose;
        string connectionString = null;
        SqlConnection connection;
        
        public SQLConnect (Queue<LogEntry> queueRef=null)
        {
            if(queueRef != null)
            {
                logQueue = queueRef;
                verbose = true;
            }

            Connect();

        }

        private void Log(string content)
        {
            if (verbose) { logQueue.Enqueue(new LogEntry("SQLConnect", content)); }
        }

        private void Connect()
        {
            Log("Starting connection SQL server connection process...");
            Log("Generating connection string...");
            connectionString = "Server=" + DATASOURCE + ";Database=" + DBNAME + ";Integrated Security=SSPI;";
            Log("Connection string is: " + connectionString);

            Log("Creating connection to SQL server...");
            connection = new SqlConnection(connectionString);
            try
            {
                Log("Validating Connection...");
                connection.Open();
                connection.Close();
                Log("Connection successfully validated!");
            }
            catch
            {
                Log("Connection failed to validate. Please review connection string.");
            }
        }
    }
}
