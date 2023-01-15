using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using System.Collections.Concurrent;
using TwitchLib.Communication.Events;
using System;
using System.Configuration;

namespace TwitchPlaysHub
{
    // Reference: https://github.com/TwitchLib/TwitchLib
    public class TwitchLibClient : ITwitchClient
    {
        public string userName;
        private string channel;
        private string lastLine;

        private TwitchClient client;

        private bool logging = false;

        private BlockingCollection<string> messageQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());

        public TwitchLibClient(string ip, int port, string userName, string password, string channel)
        {
            this.userName = userName;
            this.channel = channel;

            ConnectionCredentials credentials = new ConnectionCredentials(userName, password);
            client = new TwitchClient();
            client.Initialize(credentials, channel);

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnIncorrectLogin += Client_OnIncorrectLogin;
            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnError += Client_OnError;
            try
            {
                client.Connect();
            }
            catch (AggregateException e)
            {
                string errors = "Failed to connect:";
                foreach (Exception innerException in e.InnerExceptions)
                {
                    errors += Environment.NewLine + innerException.Message;
                }
                messageQueue.Add(errors);
            }
            catch (Exception e)
            {
                messageQueue.Add("Failed to connect: " + e.Message);
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            if (logging)
            {
                messageQueue.Add(e.Data.ToString());
            }
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            messageQueue.Add("Connection error: " + e.Error.Message);
        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            messageQueue.Add("Error: " + e.Exception.ToString());
            messageQueue.Add("Reconnecting");
            client.Reconnect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            client.JoinChannel(channel);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            messageQueue.Add("Joined channel " + e.Channel);
        }

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            messageQueue.Add("Incorrect login: " + e.Exception.ToString());
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            messageQueue.Add(e.ChatMessage.Username + ": " + e.ChatMessage.Message);
        }

        public void SendIrcMessage(string message)
        {
            // Ignored since control messages are handled by TwitchLib
        }

        public void SendPublicChatMessage(string message)
        {
            client.SendMessage(this.channel, message);
        }

        public void AttemptReconnect()
        {
            client.Connect();
        }

        public string ReadMessage()
        {
            string message = messageQueue.Take();
            lastLine = message;
            return message;
        }

        public bool IsConnected()
        {
            return client.IsConnected && client.JoinedChannels.Count > 0;
        }

        public string LastLine()
        {
            return lastLine;
        }

        public void Disconnect()
        {
            client.Disconnect();
            messageQueue.Add("Disconnected");
        }
    }
}
