using Pathfinder.Event;
using Pathfinder.ModManager;
using Pathfinder.Util;
using Command = Pathfinder.Command;

namespace HacknetIRCLink
{
    public class HacknetIRCLink : IMod
    {
        public string Identifier => "Hacknet IRC Link";

        public void Load()
        {
            Logger.Verbose("Hacknet-IRC Link loaded.");
            EventManager.RegisterListener<OSPostLoadContentEvent>(LoadIRC);
        }

        public void LoadContent()
        {
            Command.Handler.AddCommand(Commands.IRCCmd.Key, Commands.IRCCmd.IRCCommand, Commands.IRCCmd.Description, true);
            Command.Handler.AddCommand(Commands.SayCmd.Key, Commands.SayCmd.SayCommand, Commands.SayCmd.Description, true);
        }

        public void Unload()
        {
            Logger.Verbose("Unloading Hacknet_IRC Link");
            EventManager.UnregisterListener<OSPostLoadContentEvent>(LoadIRC);
        }

        public void LoadIRC(OSPostLoadContentEvent e)
        {
            Commands.IRCCmd.LoadLink(e.OS);
        }
    }
}
