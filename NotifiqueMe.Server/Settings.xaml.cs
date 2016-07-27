using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotifiqueMe.Server
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        enum DBAuthType { SSPI };

        string serverAddress;
        int serverPort = 9000;
        int maxActiveConnections = 30;

        string dbAddress = "localhost";
        string dbName = "GCAPPDB";
        DBAuthType dbAuthMode = DBAuthType.SSPI;
        string dbUsername = "";
        string dbPassword = "";

        public Settings()
        {
            InitializeComponent();
            getServerAddress();
            ServerAddressDisplay.Text = serverAddress;
            authenticationDropdown.ItemsSource = new List<string>() { "Windows (SSPI)" };
            PortInput.Text = serverPort.ToString();
            MaxConnectionsInput.Text = maxActiveConnections.ToString();
            databaseAddressInput.Text = dbAddress;
            databaseNameInput.Text = dbName;
            authenticationDropdown.SelectedItem = "Windows (SSPI)";
            usernameInput.Text = dbUsername;
            passwordInput.Text = dbPassword;
        }

        private void getServerAddress()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverAddress = ip.ToString();
                }
            }
        }
    }
}
