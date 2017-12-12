using Pathfinder.Command;
using Pathfinder.Event;
using Pathfinder.ModManager;
using Pathfinder.Util;

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
            Logger.Info("Command {0} registered.", Handler.RegisterCommand(Commands.IRCCmd.Key,
                Commands.IRCCmd.IRCCommand, Commands.IRCCmd.Description, true));
            Logger.Info("Command {0} registered.", Handler.RegisterCommand(Commands.SayCmd.Key,
                Commands.SayCmd.SayCommand, Commands.SayCmd.Description, true));
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
