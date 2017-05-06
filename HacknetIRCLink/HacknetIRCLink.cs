using System;
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
            Console.WriteLine("Hacknet-IRC Link loaded.");
        }

        public void LoadContent()
        {
            Pathfinder.Command.Handler.AddCommand("irc", Commands.IRCCommand, autoComplete: true);
        }

        public void Unload()
        {
            Console.WriteLine("Unloading Hacknet_IRC Link");
        }
        
    }
}
