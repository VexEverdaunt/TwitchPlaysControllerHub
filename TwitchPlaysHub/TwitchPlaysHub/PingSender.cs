using System.Threading;

namespace TwitchPlaysHub
{
    /*
    * Class that sends PING to irc server every 5 minutes
    */
    public class PingSender
    {
        private ITwitchClient _twitchClient;
        private Thread pingSender;

        // Empty constructor makes instance of Thread
        public PingSender(ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
            pingSender = new Thread(new ThreadStart(this.Run));
        }

        // Starts the thread
        public void Start()
        {
            pingSender.IsBackground = true;
            pingSender.Start();
        }

        // Send PING to irc server every 5 minutes
        public void Run()
        {
            while (true)
            {
                _twitchClient.SendIrcMessage("PING irc.twitch.tv");
                Thread.Sleep(300000); // 5 minutes
            }
        }
    }
}