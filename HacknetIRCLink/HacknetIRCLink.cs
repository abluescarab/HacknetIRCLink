﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hacknet;
using Pathfinder;
using Pathfinder.Util;
using Pathfinder.Event;
using Command = Pathfinder.Command;
using Executable = Pathfinder.Executable;

namespace HacknetIRCLink
{
    public class HacknetIRCLink : Pathfinder.IPathfinderMod
    {
        public string Identifier => "Hacknet IRC Link";

        public void Load()
        {
            Logger.Verbose("Hacknet-IRC Link loaded.");
            EventManager.RegisterListener<OSPostLoadContenEvent>(LoadIRC);
        }

        public void LoadContent()
        {
            Pathfinder.Command.Handler.AddCommand("irc", Commands.IRCCmd.IRCCommand, Commands.IRCCmd.Description, true);
        }

        public void Unload()
        {
            Logger.Verbose("Unloading Hacknet_IRC Link");
            EventManager.UnregisterListener<OSPostLoadContenEvent>(LoadIRC);
        }
        
        public void LoadIRC(OSPostLoadContenEvent e)
        {
            Commands.IRCCmd.LoadLink(e.OS);
        }
    }
}
