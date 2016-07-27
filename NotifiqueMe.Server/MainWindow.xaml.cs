using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
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
        // Logging variables
        public Queue<LogEntry> logQueue;
        FileStream logFileStream;
        StreamWriter logWriter;

        // Delegates for executing UI update functions.
        private delegate void updateLogCallback(string updateString);
        private delegate void updateRunningConnectionsCallback(int runningConnections);

        // UI update variables
        private bool windowUpdating;
        Thread windowUpdater;

        // Connection Manager Variables
        ConnectionManager connectionManager;
        Thread connectionManagerThread;

        // Main thread
        public MainWindow()
        {
            InitializeComponent();
            // Create the Log Queue and set the UI to update
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
        
        // Called when the start server button is clicked
        private void startServerButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable the start server button
            startServerButton.IsEnabled = false;

            // Create a test connection to validate connection to SQL server
            Log("Creating SQL test connection...");
            SQLConnect testConnection = new SQLConnect(logQueue);

            // Create a new connection manager and start it in a new thread
            Log("Initializing Connection Manager...");
            connectionManager = new ConnectionManager(logQueue);
            connectionManagerThread = new Thread(connectionManager.Listen);
            connectionManagerThread.Start();

            // Enable the close server button
            closeServerButton.IsEnabled = true;
        }

        // Called when the close server button is clicked
        private void closeServerButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable the close server button
            closeServerButton.IsEnabled = false;

            // Tell the connection manager to stop everything it's doing, then join it's thread
            Log("Stopping threads...");
            connectionManager.Stop();
            connectionManagerThread.Join();
            Log("All threads stopped successfully.");

            // Enable the start server button
            startServerButton.IsEnabled = true;
        }

        private void updateWindowThread()
        {
            // Keep track of the number of active connections last updated to.
            int lastRunningConnections = -1;

            // While the window is set to update
            while (windowUpdating == true)
            {
                // If there's a new log entry, update the log window
                if (logQueue.Count > 0)
                {
                    // Load the new log entry from the queue
                    LogEntry currEntry = logQueue.Dequeue();

                    // Generate a string from it
                    string currString = currEntry.source + " : " + currEntry.content;

                    // Invoke the log window update method
                    logWindow.Dispatcher.Invoke(new updateLogCallback(updateLog), currString);

                    // Write the line to a file
                    logWriter.WriteLine(DateTime.Now.ToString() + " : " + currString);

                }
                // If there is a connection manager running
                if (connectionManager != null)
                {
                    // And if the number of active handlers is different from the value it had
                    // last update
                    if (lastRunningConnections != connectionManager.activeHandlers.Count)
                    { 
                        // Update the last value
                        lastRunningConnections = connectionManager.activeHandlers.Count;
                        // Invoke the running connections display update method
                        currentConnectionsDisplay.Dispatcher.Invoke(new updateRunningConnectionsCallback(updateRunningConnections), lastRunningConnections);
                    }
                }
                // Sleep the thread to allow for other processes
                Thread.Sleep(100);
            }
        }

        // Method to update the log window
        private void updateLog(string updateString)
        {
            // If the log window is blank, just set it's text to the provided string
            if (logWindow.Text == "") logWindow.Text = updateString;
            // Else append the provided string to it with a new line
            else logWindow.Text = logWindow.Text + "\n" + updateString;
            logWindow.ScrollToEnd();
        }

        // Method to update the running connections display
        private void updateRunningConnections(int runningConnections)
        {
            // Update the text on the label
            currentConnectionsDisplay.Content = runningConnections + " /" + connectionManager.maxThreads;
        }

        // Logging method for this class
        private void Log(string content)
        {
            // Adds a log entry to the log queue using Main as the source
            logQueue.Enqueue(new LogEntry("Main", content));
        }

        // This method runs when the window is closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Kill the window updater thread
            windowUpdating = false;
            windowUpdater.Join();
            // Close the log file
            logFileStream.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
        }

    }
}
