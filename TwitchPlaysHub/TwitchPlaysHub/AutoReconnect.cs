using System.Threading;

namespace TwitchPlaysHub
{
    /*
    * Class that sends PING to irc server every 5 minutes
    */
    public class AutoReconnect
    {
        private IrcClient _irc;
        private Thread autoconnect;
        private int sleeptimer;

        // Empty constructor makes instance of Thread
        public AutoReconnect(IrcClient irc, int timer)
        {
            _irc = irc;
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
                
                if (_irc.IsConnected() == false)
                {
                    _irc.SendIrcMessage("Reconnecting Connection...");
                    _irc.AttemptReconnect();
                }
                Thread.Sleep(sleeptimer);
            }
        }
    }
}
