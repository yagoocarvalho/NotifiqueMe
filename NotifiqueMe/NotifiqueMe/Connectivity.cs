using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace NotifiqueMe
{
    // This class represents a singleton module to handle connections with the remote server.
    class ConnectivityModule
    {
        enum RequestType { Authenticate };

        struct ServerResponse
        {
            public RequestType responseType;
            public Dictionary<string, string> elements;

            // This parses a network packet into a server response
            public ServerResponse(string content)
            {
                elements = new Dictionary<string, string>();
                // Split the incoming string on every | separator
                string[] toProcess = content.Split('|');
                // Identify the type of response based on the first part of the string
                responseType = (RequestType) Enum.Parse(typeof(RequestType), toProcess[0]);
                // Parse the rest of the string
                for(int i = 1; i < toProcess.Length; i++)
                {
                    // Split the current string on every = separator
                    string[] processing = toProcess[i].Split('=');
                    // Add the values to the dictionary
                    elements.Add(processing[0], processing[1]);
                }
            }
        }

        // Singleton class operation variables, do not touch.
        private static ConnectivityModule instance = null;
        private static readonly object padlock = new object();

        // IP and PORT the server will be listening on.
        static string IPSTRING = "192.168.0.21";
        static int SERVERPORT = 9000;
        IPAddress serverIp;

        // Current connection state
        private bool isConnectionOpen;

        // TCP socket and network stream to read from
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream;

        // Constructor of the ConnectivityModule class.
        ConnectivityModule()
        {
            // Parse the server IP provided in the IP string and store it as a binary IP address
            serverIp = IPAddress.Parse(IPSTRING);
        }

        // Method to start a connection to the server
        public bool createConnection()
        {
            try
            {
                // Try to create a connection to the server
                clientSocket.Connect(serverIp, SERVERPORT);
                // If successfull, set the connection state to open and return true
                isConnectionOpen = true;
                return true;
            }
            catch
            {
                // If the connection fails, set the connection state to closed and return false
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

        // Method to send a message to the server
        public string sendMessage(string message)
        {
            // If the connection isn't open, try to open it
            if (!isConnectionOpen) createConnection();
            // If the connection failed to open, return an empty string 
            // (change this later to return an exception)
            if (!isConnectionOpen) return "";

            // Assign the socket stream to the network reading stream
            NetworkStream serverStream = clientSocket.GetStream();
            // Encode the message to send from ASCII to binary
            byte[] outStream = Encoding.ASCII.GetBytes(message);
            // Write the binary message to the network stream
            serverStream.Write(outStream, 0, outStream.Length);
            // Clear the network stream to prepare for server response
            serverStream.Flush();

            // Create a binary buffer with the size of the current message in the network stream
            byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
            // Read data from the network stream to the created buffer
            serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);
            // Convert the message in the buffer from Binary to ASCII
            string returndata = Encoding.ASCII.GetString(inStream);
            returndata = returndata.Substring(0, returndata.LastIndexOf('|'));
            // Return the server response string
            return returndata;
        }

        public bool Authenticate(string username, string passwordHash)
        {
            string packet = "";
            packet += RequestType.Authenticate.ToString() + "|";
            packet += "username=" + username + "|";
            packet += "passhash=" + passwordHash + "|";

            ServerResponse response = new ServerResponse(sendMessage(packet));

            if (response.responseType == RequestType.Authenticate)
            {
                if (response.elements.ContainsKey("success"))
                {
                    return bool.Parse(response.elements["success"]);
                }
                else return false;
            }
            else return false;
            
        }
    }


}
