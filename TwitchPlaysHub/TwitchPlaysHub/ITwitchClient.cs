using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchPlaysHub
{
    public interface ITwitchClient
    {
        void SendIrcMessage(string message);
        void SendPublicChatMessage(string message);
        void AttemptReconnect();
        string ReadMessage();
        bool IsConnected();
        string LastLine();
        void Disconnect();
    }
}
