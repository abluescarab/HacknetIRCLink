using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Hacknet;
using ChatSharp;
using Pathfinder;
using Pathfinder.Util;
using System.Text.RegularExpressions;

namespace HacknetIRCLink
{
    class Commands
    {
        static string ircCommandUsage = "Usage: irc [link/connect/disconnect/switch/s/help]";

        public static bool IRCCommand(Hacknet.OS os, List<string> args)
        {
            os.write(Environment.NewLine);
            
            string NickName = Regex.Replace(os.SaveUserAccountName, "[^\\w\\d-_]", "_");
            
            IRCLink link = IRCLink.getInstance(NickName, os);

            if (args.Count < 2)
            {
                os.write(ircCommandUsage);
                Logger.Verbose("Usage sent");
                return false;
            }
            if (args[1] == "link")
            {
                if (args.Count < 4)
                {
                    if(!string.IsNullOrEmpty(link.DefaultServer))
                    {
                        os.write("Linked to " + link.DefaultServer + " " + link.DefaultChannel);
                    }

                    os.write(Environment.NewLine);
                    os.write("Usage : irc link [SERVER] [CHANNEL]");
                    return false;
                }
                else
                {
                    if (args[3][0] != '#')
                    {
                        os.write("Channel name invalid. Did you forget the #?");
                        return false;
                    }
                    link.LinkServer(args[2], args[3]);
                    os.write("Server and channel set.");
                }
            }
            else if (args[1] == "connect")
            {
                if(args.Count > 3)
                {
                    link.Connect(args[2], args[3]);
                }
                else
                {
                    if(!link.Connect())
                    {
                        os.write("You have not specified a server or channel.");
                        return false;
                    }
                }
            }
            else if (args[1] == "disconnect")
            {
                if(!link.Disconnect())
                {
                    os.write("You are already disconnected.");
                    return false;
                }
                
                os.write("IRC client closed.");
            }
            else if(args[1] == "switch")
            {
                if(args.Count < 3)
                {
                    os.write("Usage: irc switch [CHANNEL]");
                    return false;
                }

                if (args[2][0] != '#')
                {
                    os.write("Channel name invalid. Did you forget the #?");
                    return false;
                }

                if(!link.SwitchChannel(args[2]))
                {
                    os.write("You are not connected to a server.");
                    return false;
                }

                os.write("Switched to channel " + args[2]);
            }
            else if (args[1] == "s")
            {
                if (args.Count < 3)
                {
                    os.write("Usage : irc s [MESSAGE]");
                    return false;
                }
                else
                {
                    string message = args[2];
                    for (int i = 3; i < args.Count; i++)
                        message += " " + args[i];

                    if(!link.Send(message))
                    {
                        os.write("Please connect to a server using \"irc connect\" before sending a message.");
                        return false;
                    }
                }
            }
            else if (args[1] == "help")
            {
                os.write(ircCommandUsage +
                    Environment.NewLine +
                    Environment.NewLine + "    link [SERVER] [CHANNEL]" +
                    Environment.NewLine + "        link to a server" +
                    Environment.NewLine + "    connect (SERVER) (CHANNEL)" +
                    Environment.NewLine + "        connect to the linked server or a provided server" +
                    Environment.NewLine + "    disconnect" +
                    Environment.NewLine + "        disconnect from the server" +
                    Environment.NewLine + "    switch [CHANNEL]" +
                    Environment.NewLine + "        switches channels" +
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
