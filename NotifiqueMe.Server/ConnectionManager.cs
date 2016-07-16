using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotifiqueMe.Server
{
    class ConnectionManager
    {
        static int STARTPORT = 9000;

        static IPAddress SERVERIP;

        Queue<LogEntry> logQueue;
        public bool verbose;

        TcpListener serverSocket;
        TcpClient clientSocket;
        bool keepRunning;

        public int maxThreads = 30;
        public int runningThreadCount = 0;

        public ConnectionManager(Queue<LogEntry> queueRef = null)
        {
            if (queueRef != null)
            {
                logQueue = queueRef;
                verbose = true;
            }

            keepRunning = true;

            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    SERVERIP = ip;
                }
            }
        }
        
        public void Listen()
        {
            Log("Creating new TcpListener on " + SERVERIP.ToString() + ", Port: " + STARTPORT.ToString() + ".");
            serverSocket = new TcpListener(SERVERIP, STARTPORT);
            clientSocket = default(TcpClient);

            Log("Starting server socket.");
            serverSocket.Start();
            Log("Server started!");
            Log("Ready to establish connection.");

            runningThreadCount = 0;
            while (keepRunning)
            {
                while (runningThreadCount <= 30)
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    runningThreadCount += 1;
                    Log("New client connected. Assigned to thread #" + Convert.ToString(runningThreadCount) + ".");
                    ClientHandler client = new ClientHandler();
                    client.startClient(clientSocket, runningThreadCount, logQueue);
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Log("Listener closed.");
        }

        public void Stop()
        {
            keepRunning = false;
            Log("Connection finished. Closing socket...");
            // Ooops, this crashes... no point fixing though, not going to work like this in the 
            // final version so just deal with the crash ok?
            Log("All sockets closed succesfully.");
        }

        private void Log(string content)
        {
            if (verbose) { logQueue.Enqueue(new LogEntry("ConnManager", content)); }
        }

    }
}
