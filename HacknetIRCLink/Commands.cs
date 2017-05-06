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
        public static class IRCCmd
        {
            public const string Key = "irc";
            public const string Description = "Hacknet IRC client";
            const string usage = "Usage: irc [link/connect/disconnect/raw/switch/help]";
            
            public static bool IRCCommand(Hacknet.OS os, List<string> args)
            {
                string NickName = Regex.Replace(os.SaveUserAccountName, "[^\\w\\d-_]", "_");
                IRCLink link = IRCLink.getInstance(NickName, os);

                os.write(Environment.NewLine);

                if (args.Count < 2)
                {
                    os.write(usage);
                    return false;
                }
                if (args[1] == "link")
                {
                    if (args.Count < 4)
                    {
                        if(!string.IsNullOrEmpty(link.DefaultServer))
                        {
                            os.write("Linked to " + link.DefaultServer + " " + link.DefaultChannel);
                            os.write(Environment.NewLine);
                        }

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

                        SaveLink(os, args[2], args[3]);
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
                else if (args[1] == "raw")
                {
                    if (args.Count < 3)
                    {
                        os.write("Usage: irc raw [RAW MESSAGE]");
                        return false;
                    }

                    string message = string.Join(" ", args.GetRange(2, args.Count - 2).ToArray());

                    if (!link.Send(message, true))
                    {
                        os.write("Please connect to a server using \"irc connect\" before issuing a command.");
                        return false;
                    }
                }
                else if (args[1] == "help")
                {
                    os.write(usage +
                        Environment.NewLine +
                        Environment.NewLine + "    link [SERVER] [CHANNEL]" +
                        Environment.NewLine + "        link to a server" +
                        Environment.NewLine + "    connect (SERVER) (CHANNEL)" +
                        Environment.NewLine + "        connect to the linked server or a provided server" +
                        Environment.NewLine + "    disconnect" +
                        Environment.NewLine + "        disconnect from the server" +
                        Environment.NewLine + "    switch [CHANNEL]" +
                        Environment.NewLine + "        switches channels" +
                        Environment.NewLine + "    raw" +
                        Environment.NewLine + "        send a raw message to the connected channel (usually commands)" +
                        Environment.NewLine + "    help" +
                        Environment.NewLine + "        show this message");
                }
                else
                {
                    os.write(usage);
                    return false;
                }
                
                os.write(Environment.NewLine);

                return true;
            }

            private static void SaveLink(OS os, string server, string channel)
            {
                FileEntry file = GetFile(os);
                file.data = server + Environment.NewLine + channel;
            }
            
            public static void LoadLink(OS os)
            {
                FileEntry file = GetFile(os);

                if(!string.IsNullOrWhiteSpace(file.data))
                {
                    string[] data = file.data.Split('\n');
                    IRCLink.getInstance("", os).LinkServer(data[0], data[1]);
                }
            }

            private static FileEntry GetFile(OS os)
            {
                string filename = "irc-link.sys";
                Folder folder = os.thisComputer.getFolderFromPath("sys");
                FileEntry file;

                if(!folder.containsFile(filename))
                {
                    file = new FileEntry("", filename);
                    folder.files.Add(file);
                }
                else
                {
                    file = folder.searchForFile(filename);
                }

                return file;
            }
        }

        public static class SayCmd
        {
            public const string Key = "say";
            public const string Description = "Send a message to the IRC channel";
            const string usage = "Usage: say [MESSAGE]";

            public static bool SayCommand(OS os, List<string> args)
            {
                if(args.Count < 2)
                {
                    os.write(usage);
                    return false;
                }
                
                string NickName = Regex.Replace(os.SaveUserAccountName, "[^\\w\\d-_]", "_");
                IRCLink link = IRCLink.getInstance(NickName, os);

                string message = string.Join(" ", args.GetRange(1, args.Count - 1).ToArray());

                if(!link.Send(message, false))
                {
                    os.write("Please connect to a server using \"irc connect\" before sending a message.");
                    return false;
                }

                return true;
            }
        }
    }
}
