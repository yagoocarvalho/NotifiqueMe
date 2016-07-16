using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

using Xamarin.Forms;
using System.Net;

namespace NotifiqueMe
{
    class ConnectivityModule
    {
        // Singleton class operation variables, do not touch.
        private static ConnectivityModule instance = null;
        private static readonly object padlock = new object();

        const string IPSTRING = "192.168.0.21";
        const int SERVERPORT = 9000;

        private bool isConnectionOpen;
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream;
        IPAddress serverIp;

        ConnectivityModule()
        {
            serverIp = IPAddress.Parse(IPSTRING);
        }

        public bool startConnection()
        {
            try
            {
                clientSocket.Connect(serverIp, SERVERPORT);
                isConnectionOpen = true;
                return true;
            }
            catch
            {
                isConnectionOpen = false;
                return false;
            }
        }

        // Gets the singleton instance of the LanguageModule class
        public static ConnectivityModule Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConnectivityModule();
                    }
                    return instance;
                }
            }
        }

        public string sendMessage()
        {
            if (!isConnectionOpen) startConnection();
            if (!isConnectionOpen) return "";

            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes("Message from Client$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
            serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);
            string returndata = Encoding.ASCII.GetString(inStream);
            return returndata;
        }
    }
}
