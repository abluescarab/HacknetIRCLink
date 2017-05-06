using ChatSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HacknetIRCLink
{
    class IRCLink
    {
        static IRCLink instance;

        Hacknet.OS os;

        public IRCLinkState state = IRCLinkState.Uninitialized;

        string defaultServer = "";
        string defaultChannel = "";
        string connectedServer = "";
        string connectedChannel = "";
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
            defaultServer = server;
            defaultChannel = channel;
        }

        public bool Connect()
        {
            return Connect(defaultServer, defaultChannel);
        }

        public bool Connect(string ip, string channel)
        {
            Console.WriteLine(Nick);

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
            connectedServer = ip;
            connectedChannel = channel;
            client.ConnectAsync();

            return true;
        }

        public bool Disconnect()
        {
            if(state != IRCLinkState.Connected)
                return false;

            connectedServer = "";
            connectedChannel = "";
            client.Quit();
            state = IRCLinkState.Ready;

            return true;
        }

        public bool Send(string message)
        {
            if(state != IRCLinkState.Connected)
                return false;

            client.SendMessage(message, connectedChannel);
            return true;
        }
    }
}
