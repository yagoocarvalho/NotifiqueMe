using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace NotifiqueMe.Server
{
    // This class handles connections between the server program and SQL server
    class SQLConnect
    {
        public enum Table { LoginCredentials, Important };

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

        // Executes a select query in the SQL Database
        // Values are returned in the same order as provided in valuesToGet
        public List<List<string>> Select(string key, string keyValue, string[] valuesToGet, Table table)
        {

            List<List<string>> returnTable = new List<List<string>>();

            Log("Building Query...");
            string queryString = "";
            queryString += "Select ";
            for( int i = 0; i < valuesToGet.Length; i++ )
            {
                queryString += valuesToGet[i];
                if (i != valuesToGet.Length-1) queryString += ", ";
                else queryString += " ";
            }
            queryString += "from " + table.ToString() + " where " + key + " = '" + keyValue + "'";
            Log("Query String is '" + queryString + "'");


            Log("Executing query...");
            SqlCommand newQuery = new SqlCommand(queryString, connection);

            SqlDataReader dataReader;


            connection.Open();
            Log("SQL Connection Opened");

            dataReader = newQuery.ExecuteReader();
            Log("Reading data from SQL connection...");

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    List<string> newRow = new List<string>(valuesToGet.Length);
                    for (int i = 0; i < valuesToGet.Length; i++)
                    {
                        newRow.Add(dataReader.GetString(i));
                        Log("Loaded " + valuesToGet[i] + " : " + newRow[i]);
                    }
                    returnTable.Add(newRow);
                }
            }
            dataReader.Close();

            connection.Close();
            Log("SQL Connection Closed");

            return returnTable;
        }

    }
}
