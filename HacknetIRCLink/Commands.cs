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
        public static bool IRCCommand(Hacknet.OS os, List<string> args)
        {
            string[] arg = args.ToArray();
            IRCLink link = IRCLink.getInstance(HacknetIRCLink.SaveNick, os);

            if (arg.Length < 2)
            {
                os.write("Usage : irc [link/send/start/stop]. Type irc help for a guide.");
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
            else if (arg[1] == "start")
            {
                if (link.state != IRCLink.IRCLinkState.Ready)
                {
                    os.write("Please use irc link to set the server and channel first.");
                    return false;
                }
                link.Connect();

            }
            else if (arg[1] == "stop")
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

            else if (arg[1] == "send")
            {
                if (link.state != IRCLink.IRCLinkState.Connected)
                {
                    os.write("Please connect to a server using irc start before sending a message.");
                    return false;
                }
                if (arg.Length < 3)
                {
                    os.write("Usage : irc send [MESSAGE]");
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
                os.write("To set the server and channel use 'irc link [SERVER] [CHANNEL]'");
                os.write("Then to start the client use 'irc start'");
                os.write("To talk use 'irc send [MESSAGE]'");
                os.write("Once you're done, use 'irc stop' to close the client.");
            }

            else
            {
                os.write("Usage : irc [link/send/start/stop]. Type irc help for a guide.");
            }
            return false;

        }
    }
}
