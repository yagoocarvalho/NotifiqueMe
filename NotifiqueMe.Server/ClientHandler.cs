using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotifiqueMe.Server
{
    class ClientHandler
    {
        TcpClient clientSocket;
        int clientCode;
        DateTime lastReceived;
        bool verbose = false;
        Queue<LogEntry> logQueue;

        public void startClient(TcpClient inClientSocket, int inClientCode, Queue<LogEntry> queueRef = null)
        {
            clientSocket = inClientSocket;
            clientCode = inClientCode;
            lastReceived = DateTime.Now;
            if (queueRef != null)
            {
                logQueue = queueRef;
                verbose = true;
            }

            Thread ctThread = new Thread(Handle);
            ctThread.Start();
        }

        public void Handle()
        {

            int requestCount = 0;
            byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Log("Received message from client: " + dataFromClient);

                    rCount = Convert.ToString(requestCount);
                    serverResponse = "Server to client(" + clientCode + ") " + rCount;
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Log("Sent message to client: " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }

        private void Log(string content)
        {
            if (verbose) { logQueue.Enqueue(new LogEntry("ClientHandler"+clientCode.ToString(), content)); }
        }
    }
}
