using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NotifiqueMe.Server
{
    // This class handles communications between the server and a single client
    class ClientHandler
    {
        enum RequestType { Authenticate };

        struct Message
        {
            public RequestType messageType;
            public Dictionary<string, string> elements;

            // This parses a network packet into a server response
            public Message(string content)
            {
                elements = new Dictionary<string, string>();
                // Split the incoming string on every | separator
                string[] toProcess = content.Split('|');
                // Identify the type of response based on the first part of the string
                messageType = (RequestType)Enum.Parse(typeof(RequestType), toProcess[0]);
                // Parse the rest of the string
                for (int i = 1; i < toProcess.Length; i++)
                {
                    // Split the current string on every = separator
                    string[] processing = toProcess[i].Split('=');
                    // Add the values to the dictionary
                    elements.Add(processing[0], processing[1]);
                }
            }
        }

        // The socket currently being used by this connection
        TcpClient clientSocket;

        // The SQL connection being used by this task
        SQLConnect sqlConnect;

        // The reference code to this client (unnecessary?)
        int clientCode;

        // When the last message was received by this handler. Will be used to timeout the handler
        DateTime lastReceived;

        // Logging variables
        bool verbose = false;
        Queue<LogEntry> logQueue;

        // Whether the handler should keep running. Set this to false to stop it.
        // Warning! Does not close open connections, only the main thread. 
        // Use ClientHandler.Stop() instead.
        bool keepRunning = true;

        // The thread this class is running on
        public Thread clientThread;

        // The ConnectionManager tracking this handler
        ConnectionManager manager;

        // Initializes a new client handler
        public void startClient(TcpClient inClientSocket, int inClientCode, ConnectionManager managerRef, Queue<LogEntry> queueRef = null)
        {
            // Set the manager of this handler to the provided reference
            manager = managerRef;
            // Set the socket in use to the provided reference
            clientSocket = inClientSocket;
            // Set the client code to the provided code
            clientCode = inClientCode;
            // Set the last interaction time to the current time
            lastReceived = DateTime.Now;
            // If a log queue was provided, set this class to generate log entries
            if (queueRef != null)
            {
                logQueue = queueRef;
                verbose = true;
            }
            // Set the main interaction loop to keep running
            keepRunning = true;
            // Create an SQL connection handler for this object to use
            sqlConnect = new SQLConnect();
            // Create a new thread to handle server-client interaction
            clientThread = new Thread(Handle);
            // Start the thread
            clientThread.Start();
        }

        // This method handles basic server-client interactions
        public void Handle()
        {
            // Initialize a variable to keep track of the number of requests served
            int requestCount = 0;
            // Create an empty string to hold the message received from the client
            string dataFromClient = null;
            // Create a binary buffer to write outgoing messages to
            byte[] sendBytes = null;
            // Create an empty string to hold the response to send to the client
            string serverResponse = null;
            // Get the network stream from the socket being used
            NetworkStream networkStream = clientSocket.GetStream();

            // While the this loop is supposed to keep running
            while (keepRunning)
            {
                try
                {
                    if (clientSocket.ReceiveBufferSize > 0)
                    {
                        // Increment requestCount by 1
                        requestCount++;
                        // Create a binary buffer with the size of the current message waiting in the socket
                        byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                        // Read the message currently waiting in the network stream
                        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        // Convert the message received from Binary to ASCII
                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        // Crop the message received to the end of message marker
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOf('|'));

                        // Create a new response to send to the client
                        serverResponse = HandleIncoming(dataFromClient);
                        // Convert the message from ASCII to Binary
                        sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        // Write the binary message to the network stream
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        // Clear the network stream
                        networkStream.Flush();
                        Log("Sent message to client: " + serverResponse);
                    }
                    Thread.Sleep(100);
                }
                catch
                {
                    // If an exception is triggered, finish the handler similar to the stop method.
                    Log("Client triggered an exception, closing socket.");
                    keepRunning = false;
                }
            }
            clientSocket.Close();
            ReportFinished();
            Log("Client handler stopped successfully.");
        }
        private string HandleIncoming(string receivedString)
        {
            Log("Received message from client: " + receivedString);
            Message receivedMessage = new Message(receivedString);
            string packet = "";

            switch (receivedMessage.messageType)
            {
                case RequestType.Authenticate:
                    Log("Message is of type '" + receivedMessage.messageType.ToString() + "'.");
                    Log("Building response packet...");
                    packet += RequestType.Authenticate.ToString() + "|";
                    packet += "success=" + Authenticate(
                        receivedMessage.elements["username"],
                        receivedMessage.elements["passhash"]) + "|";
                    break;
            }

            return packet;
        }

        // This method stops the client handler and closes the socket it's using
        public void Stop()
        {
            Log("Attempting to stop client handler.");
            // Set the variable to keep running to false to allow the main loop to exit
            keepRunning = false;
            // Close the socket being used
            clientSocket.Close();
            // Terminate the thread
            clientThread.Join();
            // Report handler conclusion
            ReportFinished();
        }

        // This method reports to the manager that the handler has finished
        void ReportFinished()
        {
            // Call the manager to remove this handler from the list of active handlers
            manager.HandlerIsDone(this);
        }

        // Logging method for this class.
        private void Log(string content)
        {
            // Adds a log entry to the log queue using ClientHandler# as the source
            if (verbose) { logQueue.Enqueue(new LogEntry("ClientHandler"+clientCode.ToString(), content)); }
        }

        // This method authenticates a user with the database
        private bool Authenticate(string username, string passwordHash)
        {
            // Create a new list to hold the result of the sql query and executes the query
            List<List<string>> sqlResult = sqlConnect.Select(
                "username", username,
                new string[] { "passhash" }, 
                SQLConnect.Table.LoginCredentials);

            // If the query returned more than one user with the same username or no matching names log an error.
            if (sqlResult.Count != 1) Log("Error: Query did not return an appropriate number of users to validate with. Number returned: " + sqlResult.Count.ToString());
            else
            {
                // Otherwise, if the returned password hash and the provided hash match,
                // confirm the authentication.
                if (sqlResult[0][0] == passwordHash) return true;
                else return false;
            }
            return false;
        }

    }
}
