using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NotifiqueMe.Server
{
    // This class manages listens to and manages all incoming connections.
    class ConnectionManager
    {
        // Port the server listens on
        static int STARTPORT = 9000;
        // IPAddress of the server
        static IPAddress SERVERIP;

        // Logging Variables
        Queue<LogEntry> logQueue;
        public bool verbose;

        // Server socket variables
        TcpListener serverSocket;
        TcpClient clientSocket;

        // Whether the manager should keep running. Set this to false to stop it.
        // Warning! Does not close open connections, only the main thread. 
        // Use ConnectionManager.Stop() instead.
        bool keepRunning;

        // List of currently active ClientHandlers
        public List<ClientHandler> activeHandlers;

        // Maximum number of running threads and current connection count
        public int maxThreads = 30;
        public int connectionCount = 0;

        // Constructor for the ConnectionManager class
        public ConnectionManager(Queue<LogEntry> queueRef = null)
        {
            // If a log queue reference was passed, set this class to produce log entries to it
            if (queueRef != null)
            {
                logQueue = queueRef;
                verbose = true;
            }

            // Set the main loop to keep running
            keepRunning = true;

            // Get the ip address of the computer running the server
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    SERVERIP = ip;
                }
            }

            // Initialize the list of active ClientHandlers.
            activeHandlers = new List<ClientHandler>();
        }
        
        // This method is the main loop of the ConnectionManager class. It listens for incoming
        // connections then delegates them to Client Handlers
        public void Listen()
        {
            // Create a server socket on the provided IP and PORT and an empty client socket
            Log("Creating new TcpListener on " + SERVERIP.ToString() + ", Port: " + STARTPORT.ToString() + ".");
            serverSocket = new TcpListener(SERVERIP, STARTPORT);
            clientSocket = default(TcpClient);

            // Initializes the server socket
            Log("Starting server socket.");
            serverSocket.Start();
            Log("Server started!");
            Log("Ready to establish connection.");

            // Initializes the current connection count
            connectionCount = 0;

            // While this is supposed to keep looping
            while (keepRunning)
            {
                // If there's a pending connection
                if (serverSocket.Pending())
                {
                    // And if the number of active handlers is currently under the maximum allowed
                    if (activeHandlers.Count < maxThreads)
                    {
                        // Accept the incoming connection
                        clientSocket = serverSocket.AcceptTcpClient();
                        // Increment the connection counter
                        connectionCount += 1;
                        Log("New client connected. Assigned to thread #" + Convert.ToString(connectionCount) + ".");
                        // Initialize a new client handler to handle the new connection
                        ClientHandler client = new ClientHandler();
                        // Add the new handler to the list of active handlers
                        activeHandlers.Add(client);
                        // Start the new client handler
                        client.startClient(clientSocket, connectionCount, this, logQueue);
                    }
                    // If the current number of active handlers has reached the maximum allowed print a log entry
                    else Log("Maximum running threads reached. Cannot create more connections.");
                }
                // If no connections are pending, sleep the thread to allow for other tasks
                else Thread.Sleep(100);
            }
            // If no longer supposed to be running, stop the server socket
            serverSocket.Stop();
            Log("Listener closed.");
        }

        // This method stops all activity in the connection manager and client handlers
        public void Stop()
        {
            // Set the keep running variable to false so the thread will finish
            keepRunning = false;
            Log("Connection finished. Closing socket...");

            // While there are items in the list of active handlers
            while (activeHandlers.Count > 0)
            {
                // Loop through the list of active handlers and call their stop methods
                for (int i = 0; i < activeHandlers.Count; i++) activeHandlers[0].Stop();
            }
            Log("All sockets closed succesfully.");
        }

        // This method is to be called by a client handler when it is no longer needed.
        public void HandlerIsDone(ClientHandler caller)
        {
            // Removes the client handler from the list of active handlers
            activeHandlers.Remove(caller);
        }

        // Logging method for this class.
        private void Log(string content)
        {
            // Adds a log entry to the log queue using ConnManager as the source
            if (verbose) { logQueue.Enqueue(new LogEntry("ConnManager", content)); }
        }

    }
}
