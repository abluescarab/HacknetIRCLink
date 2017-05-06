using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Hacknet;
using ChatSharp;
using System.Text.RegularExpressions;

namespace HacknetIRCLink
{
    class Commands
    {
        static string ircCommandUsage = "Usage: irc [link/connect/disconnect/s/help]";

        public static bool IRCCommand(Hacknet.OS os, List<string> args)
        {
            os.write(Environment.NewLine);

            string[] arg = args.ToArray();
            string NickName = Regex.Replace(os.SaveUserAccountName, "[^\\w\\d-_]", "_");
            
            IRCLink link = IRCLink.getInstance(NickName, os);

            if (arg.Length < 2)
            {
                os.write(ircCommandUsage);
                Console.WriteLine("Usage sent");
                return false;
            }
            if (arg[1] == "link")
            {
                if (arg.Length < 4)
                {
                    os.write("Usage : irc link [SERVER] [CHANNEL]");
                    return false;
                }
                else
                {
                    if (arg[3][0] != '#')
                    {
                        os.write("Channel name invalid. Did you forget the #?");
                        return false;
                    }
                    link.LinkServer(arg[2], args[3]);
                    os.write("Server and channel set.");
                }
            }
            else if (arg[1] == "connect")
            {
                if(link.state != IRCLink.IRCLinkState.Ready)
                {
                    os.write("Please use \"irc link\" to set the server and channel first.");
                    return false;
                }
                link.Connect();
            }
            else if (arg[1] == "disconnect")
            {
                if (link.state != IRCLink.IRCLinkState.Connected)
                {
                    os.write("You are already disconnected.");
                    return false;
                }
                link.Disconnect();
                os.write("IRC client closed.");
                return false;
            }

            else if (arg[1] == "s")
            {
                if (link.state != IRCLink.IRCLinkState.Connected)
                {
                    os.write("Please connect to a server using \"irc connect\" before sending a message.");
                    return false;
                }
                if (arg.Length < 3)
                {
                    os.write("Usage : irc s [MESSAGE]");
                    return false;
                }
                else
                {
                    string message = arg[2];
                    for (int i = 3; i < arg.Length; i++)
                        message += " " + arg[i];
                    link.Send(message);
                }
            }
            else if (arg[1] == "help")
            {
                os.write(
                    ircCommandUsage +
                    Environment.NewLine + "    link [SERVER] [CHANNEL]" +
                    Environment.NewLine + "        link to a server" +
                    Environment.NewLine + "    connect (SERVER) (CHANNEL)" +
                    Environment.NewLine + "        connect to the linked server or a provided server" +
                    Environment.NewLine + "    disconnect" +
                    Environment.NewLine + "        disconnect from the server" +
                    Environment.NewLine + "    s" +
                    Environment.NewLine + "        send a message to the connected channel" +
                    Environment.NewLine + "    help" +
                    Environment.NewLine + "        show this message");
            }

            else
            {
                os.write(ircCommandUsage);
                return false;
            }

            os.write(Environment.NewLine);

            return true;
        }
    }
}
