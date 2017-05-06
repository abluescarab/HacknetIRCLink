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

        string server = "";
        string channelname = "";
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
            if (instance == null)
                instance = new IRCLink(nickname, os);
            return instance;
        }


        public void LinkServer(string server, string channel)
        {
            this.server = server;
            this.channelname = channel;
            state = IRCLinkState.Ready;
        }


        public void Connect()
        {
            if (state != IRCLinkState.Ready)
                return;
            client = new IrcClient(server, new IrcUser(Nick, "HacknetLink"));

            client.ConnectionComplete += (s, e) => client.JoinChannel(channelname);

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
            client.ConnectAsync();

        }

        public void Disconnect()
        {
            if (state != IRCLinkState.Connected)
                return;

            client.Quit();
            state = IRCLinkState.Ready;
        }

        public void Send(string message)
        {
            if (state != IRCLinkState.Connected)
                return;
            client.SendMessage(message, channelname);
        }


    }
}
