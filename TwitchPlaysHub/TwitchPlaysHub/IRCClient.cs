using System;
using System.Net.Sockets;
using System.IO;

namespace TwitchPlaysHub
{
    // Reference: https://www.youtube.com/watch?v=Ss-OzV9aUZg
    public class IrcClient
    {
        public string userName;
        private string channel;
        private string password;
        private string lastLine;

        private TcpClient _tcpClient;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;

        public IrcClient(string ip, int port, string userName, string password, string channel)
        {
            try
            {
                this.userName = userName;
                this.channel = channel;
                this.password = password;

                _tcpClient = new TcpClient(ip, port);
                _inputStream = new StreamReader(_tcpClient.GetStream());
                _outputStream = new StreamWriter(_tcpClient.GetStream());

                // Try to join the room
                _outputStream.WriteLine("PASS " + password);
                _outputStream.WriteLine("NICK " + userName);
                _outputStream.WriteLine("USER " + userName + " 8 * :" + userName);
                _outputStream.WriteLine("JOIN #" + channel);
                _outputStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendIrcMessage(string message)
        {
            try
            {
                _outputStream.WriteLine(message);
                _outputStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendPublicChatMessage(string message)
        {
            try
            {
                SendIrcMessage(":" + userName + "!" + userName + "@" + userName +
                ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AttemptReconnect()
        {
            try
            {
                _outputStream.WriteLine("PASS " + password);
                _outputStream.WriteLine("NICK " + userName);
                _outputStream.WriteLine("USER " + userName + " 8 * :" + userName);
                _outputStream.WriteLine("JOIN #" + channel);
                _outputStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string ReadMessage()
        {
            try
            {
                string message = _inputStream.ReadLine();
                lastLine = message;
                return message;
            }
            catch (Exception ex)
            {
                return "Error receiving message: " + ex.Message;
            }
        }

        public bool IsUpdated()
        {
            if (_inputStream.ReadLine() != lastLine) {
                return true;
            }
            return false;
        }

        public bool IsConnected()
        {
            if (_tcpClient.Connected)
            {
                return true;
            }

            /*
            if (_tcpClient != null)
            {
                return true;
            }
            if (_tcpClient.Client.Connected)
            {
                return true;
            }
            
            */
            return false;
        }

        public string LastLine()
        {
            return lastLine;
        }
    }
}