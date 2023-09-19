using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchPlaysHub
{
    public interface IMessageParser
    {
        void Parse(string message);
        bool IsValid();
        string GetChatMessage();
        string GetUserName();
    }

    public class IrcMessageParser : IMessageParser
    {
        private string userName;
        private string chatMessage;
        private bool valid;

        public IrcMessageParser()
        {
            userName = "";
            chatMessage = "";
            valid = false;
        }

        public void Parse(string message)
        {
            userName = "";
            chatMessage = "";
            valid = false;

            if (message.Contains("PRIVMSG"))
            {
                // Messages from the users will look something like this (without quotes):
                // Format: ":[user]![user]@[user].tmi.twitch.tv PRIVMSG #[channel] :[message]"

                // Modify message to only retrieve user and message
                int intIndexParseSign = message.IndexOf('!');
                userName = message.Substring(1, intIndexParseSign - 1); // parse username from specific section (without quotes)
                                                                               // Format: ":[user]!"
                                                                               // Get user's message
                intIndexParseSign = message.IndexOf(" :");
                chatMessage = message.Substring(intIndexParseSign + 2);

                valid = true;
            }
        }

        public bool IsValid()
        {
            return valid;
        }

        public string GetChatMessage()
        {
            return chatMessage;
        }

        public string GetUserName()
        {
            return userName;
        }
    }

    public class TwitchLibClientMessageParser : IMessageParser
    {
        private string userName;
        private string chatMessage;
        private bool valid;

        public TwitchLibClientMessageParser()
        {
            userName = "";
            chatMessage = "";
            valid = false;
        }

        public void Parse(string message)
        {
            valid = false;
            userName = "";
            chatMessage = "";

            if (message.Contains(": "))
            {
                userName = message.Substring(0, message.IndexOf(':'));
                chatMessage = message.Substring(message.IndexOf(": ") + 2);
                valid = true;
            }
        }

        public bool IsValid()
        {
            return valid;
        }

        public string GetChatMessage()
        {
            return chatMessage;
        }

        public string GetUserName()
        {
            return userName;
        }
    }
}
