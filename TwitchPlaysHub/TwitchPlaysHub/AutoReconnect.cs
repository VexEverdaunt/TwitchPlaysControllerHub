using System.Threading;

namespace TwitchPlaysHub
{
    /*
    * Class that sends PING to irc server every 5 minutes
    */
    public class AutoReconnect
    {
        private ITwitchClient _twitchClient;
        private Thread autoconnect;
        private int sleeptimer;

        // Empty constructor makes instance of Thread
        public AutoReconnect(ITwitchClient twitchClient, int timer)
        {
            _twitchClient = twitchClient;
            sleeptimer = timer;
            autoconnect = new Thread(new ThreadStart(this.Run));
        }

        // Starts the thread
        public void Start()
        {
            autoconnect.IsBackground = true;
            autoconnect.Start();
        }

        // Send PING to irc server every 5 minutes
        public void Run()
        {
            while (true)
            {
                
                if (_twitchClient.IsConnected() == false)
                {
                    _twitchClient.SendIrcMessage("Reconnecting Connection...");
                    _twitchClient.AttemptReconnect();
                }
                Thread.Sleep(sleeptimer);
            }
        }
    }
}
