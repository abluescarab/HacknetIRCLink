using System.Linq;
using System.Text.RegularExpressions;
using ChatSharp;
using ChatSharp.Events;
using Hacknet;
using Pathfinder.Util;

namespace HacknetIRCLink
{
    class IRCLink
    {
        public enum IRCLinkState
        {
            Uninitialized,
            Ready,
            Connected
        }

        private static IRCLink instance;

        private OS os;
        private IrcClient client;
        private string nickname = "";

        public IRCLinkState state = IRCLinkState.Uninitialized;

        public string DefaultServer { get; private set; }
        public string DefaultChannel { get; private set; }
        public string ConnectServer { get; private set; }
        public string ConnectedChannel { get; private set; }

        IRCLink(string nickname, OS os)
        {
            this.nickname = nickname;
            this.os = os;
        }

        public static IRCLink GetInstance(string nickname, OS os)
        {
            if (instance == null)
            {
                instance = new IRCLink(nickname, os);
            }
            else
            {
                instance.nickname = nickname;
                instance.os = os;
            }

            if (instance.state == IRCLinkState.Uninitialized)
            {
                instance.state = IRCLinkState.Ready;
            }

            return instance;
        }
        
        public void LinkServer(string server, string channel)
        {
            DefaultServer = server;
            DefaultChannel = channel;
        }

        public bool Connect()
        {
            return Connect(DefaultServer, DefaultChannel);
        }

        public bool Connect(string ip, string channel)
        {
            Logger.Verbose(nickname);

            if (state != IRCLinkState.Ready)
            {
                return false;
            }

            client = new IrcClient(ip, new IrcUser(nickname, "HacknetLink"));

            client.ConnectionComplete += (s, e) => client.JoinChannel(channel);

            client.UserJoinedChannel += (s, e) =>
            {
                os.write("User " + e.User.Nick + " has joined the channel.");
            };

            client.UserPartedChannel += (s, e) =>
            {
                os.write("User " + e.User.Nick + " has left the channel.");
            };
            
            client.NetworkError += (s, e) =>
            {
                if (state != IRCLinkState.Ready)
                    os.write("Connection error. Check your internet connection and the server details.");
            };

            client.ChannelMessageRecieved += (s, e) =>
            {
                string SenderNick = Regex.Split(e.PrivateMessage.User.Nick, "!")[0];
                string message = e.PrivateMessage.Message;
                os.write(SenderNick + ": " + message);
            };

            state = IRCLinkState.Connected;
            ConnectServer = ip;
            ConnectedChannel = channel;
            client.ConnectAsync();

            return true;
        }

        public bool SwitchChannel(string channel)
        {
            if (state != IRCLinkState.Connected)
            {
                return false;
            }
            
            IrcChannel connected = client.Channels.FirstOrDefault(c => c.Name.ToLower().Equals(ConnectedChannel));

            if (connected != null)
            {
                client.PartChannel(connected.Name);
            }

            client.JoinChannel(channel);
            ConnectedChannel = channel;
            return true;
        }

        public bool Disconnect()
        {
            if (state != IRCLinkState.Connected)
            {
                return false;
            }

            ConnectServer = "";
            ConnectedChannel = "";
            client.Quit();
            state = IRCLinkState.Ready;

            return true;
        }

        public bool Send(string message, bool raw)
        {
            if (state != IRCLinkState.Connected)
            {
                return false;
            }
            
            if (raw)
            {
                client.RawMessageRecieved += Client_RawMessageReceived;
                client.SendRawMessage(message, ConnectedChannel);
            }
            else
            {
                client.SendMessage(message, ConnectedChannel);
                //os.write(Nick + ": " + message);
            }

            return true;
        }

        private void Client_RawMessageReceived(object sender, RawMessageEventArgs e)
        {
            os.write(e.Message);
            client.RawMessageRecieved -= Client_RawMessageReceived;
        }
    }
}
