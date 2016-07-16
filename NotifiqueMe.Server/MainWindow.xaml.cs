using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace NotifiqueMe.Server
{
    public struct LogEntry
    {
        public string source;
        public string content;

        public LogEntry(string owner, string message)
        {
            source = owner;
            content = message;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Queue<LogEntry> logQueue;
        private delegate void updateLogCallback(string updateString);
        private delegate void updateRunningConnectionsCallback(int runningConnections);

        private bool windowUpdating;
        Thread windowUpdater;
        ConnectionManager connectionManager;
        Thread connectionManagerThread;

        FileStream logFileStream;
        StreamWriter logWriter;

        public MainWindow()
        {
            InitializeComponent();
            // Create the Log Queue and set it to update
            logQueue = new Queue<LogEntry>();
            windowUpdating = true;

            // Create a thread to update the log window
            windowUpdater = new Thread(updateWindowThread);

            // Open logfile to write to
            logFileStream = File.Open("serverlog.txt", FileMode.Append);
            logWriter = new StreamWriter(logFileStream);
            logWriter.AutoFlush = true;

            // Start the log updater thread
            windowUpdater.Start();
        }

        private void startServerButton_Click(object sender, RoutedEventArgs e)
        {
            startServerButton.IsEnabled = false;
            closeServerButton.IsEnabled = true;

            Log("Creating SQL test connection...");
            SQLConnect testConnection = new SQLConnect(logQueue);
            Log("Initializing Connection Manager...");
            connectionManager = new ConnectionManager(logQueue);
            connectionManagerThread = new Thread(connectionManager.Listen);
            connectionManagerThread.Start();
        }

        private void closeServerButton_Click(object sender, RoutedEventArgs e)
        {
            startServerButton.IsEnabled = true;
            closeServerButton.IsEnabled = false;

            Log("Stopping threads...");
            connectionManager.Stop();
            connectionManagerThread.Join();
            Log("All threads stopped successfully.");
        }

        private void updateWindowThread()
        {
            int lastRunningConnections = -1;

            while (windowUpdating == true)
            {
                
                if (logQueue.Count > 0)
                {
                    LogEntry currEntry = logQueue.Dequeue();
                    string currString = currEntry.source + " : " + currEntry.content;

                    logWindow.Dispatcher.Invoke(new updateLogCallback(updateLog), currString);

                    logWriter.WriteLine(DateTime.Now.ToString() + " : " + currString);

                }
                if (connectionManager != null)
                {
                    if (lastRunningConnections != connectionManager.runningThreadCount)
                    {
                        lastRunningConnections = connectionManager.runningThreadCount;
                        currentConnectionsDisplay.Dispatcher.Invoke(new updateRunningConnectionsCallback(updateRunningConnections), lastRunningConnections);
                    }
                }
            }
            windowUpdater.Join();
        }

        private void updateLog(string updateString)
        {
            if (logWindow.Text == "") logWindow.Text = updateString;
            else logWindow.Text = logWindow.Text + "\n" + updateString;
        }

        private void updateRunningConnections(int runningConnections)
        {
            currentConnectionsDisplay.Content = runningConnections + " /" + connectionManager.maxThreads;
        }

        private void Log(string content)
        {
            logQueue.Enqueue(new LogEntry("Main", content));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowUpdating = false;
            logFileStream.Close();
            while (windowUpdater.IsAlive) { }
        }
    }
}
