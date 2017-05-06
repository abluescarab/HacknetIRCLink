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
            string NickName = os.SaveUserAccountName;
            NickName = NickName.Replace('(', '_');
            NickName = NickName.Replace(')', '_');
            NickName = NickName.Replace('+', '_');
            NickName = NickName.Replace('*', '_');
            NickName = NickName.Replace('\"', '_');
            NickName = NickName.Replace('\'', '_');
            NickName = NickName.Replace('\\', '_');
            NickName = NickName.Replace('/', '_');

            IRCLink link = IRCLink.getInstance(NickName, os);

            if (arg.Length < 2)
            {
                os.write(Environment.NewLine + "Usage : irc [link/s/start/stop]. Type irc help for a guide." + Environment.NewLine);
                Console.WriteLine("Usage sent");
                return false;
            }
            if (arg[1] == "link")
            {
                if (arg.Length < 4)
                {
                    os.write(Environment.NewLine + "Usage : irc link [SERVER] [CHANNEL]" + Environment.NewLine);
                    return false;
                }
                else
                {
                    if (arg[3][0] != '#')
                    {
                        os.write(Environment.NewLine + "Channel name invalid. Did you forget the #?" + Environment.NewLine);
                        return false;
                    }
                    link.LinkServer(arg[2], args[3]);
                    os.write(Environment.NewLine + "Server and channel set." + Environment.NewLine);
                }
            }
            else if (arg[1] == "start")
            {
                if (link.state != IRCLink.IRCLinkState.Ready)
                {
                    os.write(Environment.NewLine + "Please use irc link to set the server and channel first." + Environment.NewLine);
                    return false;
                }
                link.Connect();

            }
            else if (arg[1] == "stop")
            {
                if (link.state != IRCLink.IRCLinkState.Connected)
                {
                    os.write(Environment.NewLine + "You are already disconnected." + Environment.NewLine);
                    return false;
                }
                link.Disconnect();
                os.write(Environment.NewLine + "IRC client closed." + Environment.NewLine);
                return false;
            }

            else if (arg[1] == "s")
            {
                if (link.state != IRCLink.IRCLinkState.Connected)
                {
                    os.write(Environment.NewLine + "Please connect to a server using irc start before sending a message." + Environment.NewLine);
                    return false;
                }
                if (arg.Length < 3)
                {
                    os.write(Environment.NewLine + "Usage : irc s [MESSAGE]" + Environment.NewLine);
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
                os.write(Environment.NewLine + "To set the server and channel use 'irc link [SERVER] [CHANNEL]'");
                os.write("Then to start the client use 'irc start'");
                os.write("To talk use 'irc s [MESSAGE]'");
                os.write("Once you're done, use 'irc stop' to close the client."+ Environment.NewLine);
            }

            else
            {
                os.write(Environment.NewLine + "Usage : irc [link/s/start/stop]. Type irc help for a guide." + Environment.NewLine);
            }
            return false;

        }
    }
}
