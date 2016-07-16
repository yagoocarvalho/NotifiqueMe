using System.Collections.Generic;
using System.Data.SqlClient;

namespace NotifiqueMe.Server
{
    // This class handles connections between the server program and SQL server
    class SQLConnect
    {
        // Constants to set up connection string. Might become statics later?
        private const string DATASOURCE = "localhost";
        private const string DBNAME = "GCAPPDB";

        // Logging variables
        Queue<LogEntry> logQueue;
        public bool verbose;

        // SQL connection variables
        string connectionString = null;
        SqlConnection connection;
        
        // Constructor of the SQLConnect class
        public SQLConnect (Queue<LogEntry> queueRef=null)
        {
            // If a log queue reference was passed, set the class to log it's tasks
            if(queueRef != null)
            {
                logQueue = queueRef;
                verbose = true;
            }

            // Call a method to handle the connection. (Move this out of constructor later)
            Connect();

        }

        // This method handles logging calls from this class
        private void Log(string content)
        {
            // Add a new log entry to the queue with SQLConnect as the source
            if (verbose) { logQueue.Enqueue(new LogEntry("SQLConnect", content)); }
        }

        // This method handles the initial connection to SQL server
        private void Connect()
        {
            Log("Starting connection SQL server connection process...");
            // Generate a connection string from the entered data
            // Integrated Security uses the local windows authentication
            // This will be made more variable later
            Log("Generating connection string...");
            connectionString = "Server=" + DATASOURCE + ";Database=" + DBNAME + ";Integrated Security=SSPI;";
            Log("Connection string is: " + connectionString);

            // Create a new SqlConnection object
            Log("Creating connection to SQL server...");
            connection = new SqlConnection(connectionString);
            try
            {
                // Attempt to connect to the server.
                // This is just a test for now. Connection should be maintained once there's
                // something to do with it.
                Log("Validating Connection...");
                connection.Open();
                connection.Close();
                Log("Connection successfully validated!");
            }
            catch
            {
                // If connection to SQL server fails, log an error
                Log("Connection failed to validate. Please review connection string.");
            }
        }
    }
}
