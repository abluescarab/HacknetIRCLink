using ChatSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pathfinder;
using Pathfinder.Util;

namespace HacknetIRCLink
{
    class IRCLink
    {
        static IRCLink instance;

        Hacknet.OS os;

        public IRCLinkState state = IRCLinkState.Uninitialized;

        public string DefaultServer { get; private set; }
        public string DefaultChannel { get; private set; }
        public string ConnectServer { get; private set; }
        public string ConnectedChannel { get; private set; }

        string Nick = "";

        IrcClient client;

        public enum IRCLinkState
        {
            Uninitialized,
            Ready,
            Connected
        }

        IRCLink(string nickname, Hacknet.OS os)
        {
            Nick = nickname;
            this.os = os;
        }

        public static IRCLink getInstance(string nickname, Hacknet.OS os)
        {
            if(instance == null)
                instance = new IRCLink(nickname, os);
            else
            {
                instance.Nick = nickname;
                instance.os = os;
            }

            if(instance.state == IRCLinkState.Uninitialized)
                instance.state = IRCLinkState.Ready;

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
            Logger.Verbose(Nick);

            if(state != IRCLinkState.Ready)
                return false;

            client = new IrcClient(ip, new IrcUser(Nick, "HacknetLink"));

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
                if(state != IRCLinkState.Ready)
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
            if(state != IRCLinkState.Connected)
                return false;
            
            IrcChannel connected = client.Channels.FirstOrDefault(c => c.Name.ToLower().Equals(ConnectedChannel));

            if(connected != null)
                client.PartChannel(connected.Name);

            client.JoinChannel(channel);
            ConnectedChannel = channel;
            return true;
        }

        public bool Disconnect()
        {
            if(state != IRCLinkState.Connected)
                return false;

            ConnectServer = "";
            ConnectedChannel = "";
            client.Quit();
            state = IRCLinkState.Ready;

            return true;
        }

        public bool Send(string message, bool raw)
        {
            if(state != IRCLinkState.Connected)
                return false;
            
            if(raw)
            {
                client.RawMessageRecieved += client_RawMessageReceived;
                client.SendRawMessage(message, ConnectedChannel);
            }
            else
            {
                client.SendMessage(message, ConnectedChannel);
                //os.write(Nick + ": " + message);
            }

            return true;
        }

        private void client_RawMessageReceived(object sender, ChatSharp.Events.RawMessageEventArgs e)
        {
            os.write(e.Message);
            client.RawMessageRecieved -= client_RawMessageReceived;
        }
    }
}
